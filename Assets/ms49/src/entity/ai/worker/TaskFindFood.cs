using UnityEngine;

/// <summary>
/// Executes if their is a table with chair that is unoccupied &&
/// the Worker is hungry && (the worker is a Cook who is carrying a
/// plate of food) OR (the worker is not a cook)
/// </summary>
public class TaskFindFood : TaskMovement<EntityWorker> {

    [SerializeField]
    private float stopEattingValue = 99;
    [SerializeField]
    private float eatSpeed = 20;
    [SerializeField]
    private float _startFoodHuntAt = 20;
    [SerializeField]
    private Sprite _emoteHungrySprite = null;
    [SerializeField]
    private Sprite _emoteExclamtionSprite = null;

    public float startFoodHuntAt => this._startFoodHuntAt;

    private CellBehaviorTable table;
    private CookMetaData cookMeta;

    protected override void initializeReferences() {
        base.initializeReferences();

        this.cookMeta = this.GetComponentInChildren<CookMetaData>();
    }

    public override bool shouldExecute() {
        if(this.owner.hunger.value <= startFoodHuntAt) {
            // Find a table.

            if(this.isCook() && this.cookMeta.plateState != CellBehaviorTable.EnumPlateState.FULL) {
                return false;
            }

            foreach(CellBehaviorTable table in this.owner.world.getAllBehaviors<CellBehaviorTable>(behavior => behavior.hasChair && !behavior.isOccupied())) {
                NavPath path = this.agent.calculatePath(table.chairPos);

                if(path != null) {
                    path.endingLookDirection = Rotation.directionToRotation(table.center - table.chair.behavior.center);

                    this.navPath = path;
                    this.table = table;
                    this.table.setOccupant(this.owner);

                    return true;
                } else {
                    this.owner.emote.startEmote(new Emote(this._emoteExclamtionSprite, 0.1f).setTooltip("Can't find a table"));
                }
            }
        }

        return false;
    }

    public override void onTaskStart() {
        base.onTaskStart();

        this.owner.emote.startEmote(new Emote(this._emoteHungrySprite, -1).setPriority().setTooltip("Hungry"));
    }

    public override bool continueExecuting() {
        return this.table != null && this.table.chair != null && this.owner.hunger.value < stopEattingValue;
    }

    public override void onDestinationArive() {
        base.onDestinationArive();

        if(this.isCook()) { // Must be a Cook
            // If they're holding a plate still, put it down
            this.table.plateState = CellBehaviorTable.EnumPlateState.FULL;
            this.cookMeta.plateState = CellBehaviorTable.EnumPlateState.NONE;
        }
    }

    public override void onAtDestination() {
        base.onAtDestination();

        if(this.table.plateState == CellBehaviorTable.EnumPlateState.FULL) {
            this.owner.emote.cancelEmote();

            this.owner.hunger.increase(this.eatSpeed * Time.deltaTime);
            if(this.owner.hunger.value >= this.stopEattingValue) {
                this.table.plateState = CellBehaviorTable.EnumPlateState.DIRTY; // TODO make the plate dirty
            }

            //this.owner.animator.playClip("Eating"); // TODO remake animation
        }
    }

    public override void onTaskStop() {
        base.onTaskStop();

        if(this.table != null) {
            this.table.setOccupant(null);

            // Try to move away from the chair so it "looks free".
            // If there no walkable space, stay on the chair.  Others will still be able to claim it.
            Position? freeSpot = this.getFreeSpot(this.owner.position);
            if(freeSpot != null) {
                NavPath path = this.agent.calculatePath(
                    (Position)freeSpot,
                    false);

                if(path != null) {
                    this.agent.setPath(path);
                }
            }

            //this.owner.animator.stopClip();
        }

        this.table = null;

        this.owner.emote.cancelEmote();
    }

    public override bool canBeInterupted() {
        return false;
    }

    private bool isCook() {
        return this.cookMeta != null;
    }
}
