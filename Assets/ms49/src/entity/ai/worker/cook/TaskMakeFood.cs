using UnityEngine;

/// <summary>
/// Executes if (there is a Worker at a table with no plate) OR (the
/// Cook is hungry) AND the Cook has a clean plate.
/// </summary>
public class TaskMakeFood : TaskMovement<EntityWorker> {

    [SerializeField]
    private float cookSpeed = 4f;
    [SerializeField]
    private CookMetaData cookData = null;
    [SerializeField]
    private Sprite _emoteStoveSprite = null;

    private CellBehaviorStove stove;
    private float cookTimer;
    private TaskFindFood taskFindFood;

    protected override void initializeReferences() {
        base.initializeReferences();

        this.taskFindFood = this.GetComponentInParent<TaskFindFood>();
    }

    public override bool shouldExecute() {
        if(this.cookData.plateState == CellBehaviorTable.EnumPlateState.CLEAN || this.cookData.plateState == CellBehaviorTable.EnumPlateState.NONE) {
            if(this.doesSomeoneNeedFood()) {
                this.stove = this.calculateAndSetPathToClosest<CellBehaviorStove>(
                    false,
                    behavior => !behavior.isOccupied(),
                    Rotation.UP,
                    Position.down);

                if(this.stove != null) {
                    this.stove.setOccupant(this.owner);
                    return true;
                } else {
                    // Someone needs food, but there is no stove
                    this.owner.emote.startEmote(new Emote(this._emoteStoveSprite, 0.1f).setTooltip("Needs a stove to cook food"));

                }
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        return
            this.stove != null &&
            this.cookData.plateState != CellBehaviorTable.EnumPlateState.FULL;
    }

    public override void onAtDestination() {
        base.onAtDestination();

        // Cook food...
        this.cookTimer += Time.deltaTime;
        if(this.cookTimer >= this.
            cookSpeed) {
            this.stove.setOccupant(null);

            this.cookData.plateState = CellBehaviorTable.EnumPlateState.FULL;
        }
    }

    public override void onTaskStop() {
        base.onTaskStop();

        if(this.stove != null) {
            this.stove.setOccupant(null);
        }

        this.stove = null;

        this.cookTimer = 0;
    }

    private bool doesSomeoneNeedFood() {
        if(this.owner.hunger.value <= this.taskFindFood.startFoodHuntAt) {
            // The Cook (owner) needs food.
            return true;
        }

        foreach(CellBehaviorTable table in this.owner.world.getAllBehaviors<CellBehaviorTable>()) {
            if(table.plateState == CellBehaviorTable.EnumPlateState.NONE && table.isOccupantSitting()) {
                // Someone else needs food.
                return true;
            }
        }

        // No one needs food
        return false;
    }
}
