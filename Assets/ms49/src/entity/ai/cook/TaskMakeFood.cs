using UnityEngine;

/// <summary>
/// Executes if there is a Worker at a table with no plate and the
/// owner has a clean plate.
/// </summary>
public class TaskMakeFood : TaskBase<EntityCook> {

    private const float FOOD_COOK_TIME = 4f;

    private CellBehaviorTable table;
    private CellBehaviorStove stove;
    private int stage; // 0 = making food, 1 = going to table
    private float cookTimer;

    public TaskMakeFood(EntityCook owner, MoveHelper moveHelper) : base(owner, moveHelper) { }

    public override bool continueExecuting() {
        if(this.table == null || !this.table.isOccupied() || this.table.plateState != CellBehaviorTable.EnumPlateState.NONE) {
            return false;
        }

        if(this.stove == null) {
            return false;
        }

        return true;
    }

    public override void preform() {
        if(this.stage == 0) {
            if(this.owner.position.distance(this.stove.pos) <= 1) {
                // cook food...
                this.cookTimer += Time.deltaTime;
                if(this.cookTimer >= FOOD_COOK_TIME) {
                    this.owner.plateState = CellBehaviorTable.EnumPlateState.FULL;
                    this.stove.setOccupant(null);
                    this.stage = 1;
                    if(this.moveHelper.setDestination(this.table.pos, true) == null) {
                        // Can no longer go to table...
                    }
                }
            }
        } else { // state == 1
            if(this.owner.position.distance(this.table.pos) <= 1) {
                this.owner.plateState = CellBehaviorTable.EnumPlateState.NONE;
                this.table.plateState = CellBehaviorTable.EnumPlateState.FULL;
            }
        }
    }

    public override void resetTask() {
        base.resetTask();

        if(this.stove != null) {
            this.stove.setOccupant(null);
        }

        this.table = null;
        this.cookTimer = 0;
    }

    public override bool shouldExecute() {
        if(this.owner.plateState == CellBehaviorTable.EnumPlateState.CLEAN) {
            foreach(CellBehaviorTable table in this.owner.world.getAllBehaviors<CellBehaviorTable>()) {
                if(table.plateState == CellBehaviorTable.EnumPlateState.NONE && table.isOccupantSitting()) {
                    // Someone needs food.

                    // Make use there is a stove, and go to it
                    bool foundStove = false;
                    foreach(CellBehaviorStove stove in this.owner.world.getAllBehaviors<CellBehaviorStove>()) {
                        if(this.moveHelper.setDestination(stove.pos + Rotation.DOWN, false) != null) {
                            foundStove = true;

                            this.stove = stove;
                            this.stove.setOccupant(this.owner);

                            this.table = table;
                            this.stage = 0;

                            return true;
                        }
                    }
                    if(!foundStove) {
                        return false; // Even though someone needs food, there is no stove
                    }


                }
            }
        }

        return false;
    }
}
