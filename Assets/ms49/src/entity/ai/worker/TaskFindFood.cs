using UnityEngine;

/// <summary>
/// Executes if their is a table with chair that is unoccupied &&
/// the Worker is hungry && (the worker is a Cook who is carrying a
/// plate of food) OR (the worker is not a cook)
/// </summary>
public class TaskFindFood : TaskMovement<EntityWorker> {

    [SerializeField]
    private float _eatSpeed = 5;
    [SerializeField]
    private float _startFoodHuntAt = 20;

    private CellBehaviorTable table;
    private CookMetaData cookMeta;

    public float startFoodHuntAt => this._startFoodHuntAt + this.owner.info.personality.eatStartOffset;
    public int stopAtValue => (int)this.owner.hunger.maxValue + this.owner.info.personality.eatStopOffset;

    protected override void initializeReferences() {
        base.initializeReferences();

        this.cookMeta = this.GetComponentInChildren<CookMetaData>();
    }

    public override bool shouldExecute() {
        if(this.owner.hunger.value <= this.startFoodHuntAt) {
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
                    this.owner.emote.startEmote(new Emote("exclamation", 0.1f).setTooltip("Can't find a table"));
                }
            }
        }

        return false;
    }

    public override void onTaskStart() {
        base.onTaskStart();

        this.owner.emote.startEmote(new Emote("food", -1).setPriority().setTooltip("Hungry"));
    }

    public override bool continueExecuting() {
        return this.table != null && this.table.chair != null && this.owner.hunger.value < this.stopAtValue;
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

            float amount = this._eatSpeed * Time.deltaTime;
            if(this.owner.info.personality.require2Meals) {
                amount /= 2;
            }

            this.owner.hunger.increase(amount);
            if(this.owner.hunger.value >= this.stopAtValue) {
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
