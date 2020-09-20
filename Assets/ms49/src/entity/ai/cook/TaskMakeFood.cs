using UnityEngine;

/// <summary>
/// Executes if there is a Worker at a table with no plate and the
/// owner has a clean plate.
/// </summary>
public class TaskMakeFood : TaskBase<EntityWorker> {

    [SerializeField]
    private float cookSpeed = 4f;
    [SerializeField]
    private CookMetaData cookData = null;

    private CellBehaviorTable table;
    private CellBehaviorStove stove;
    private int stage; // 0 = making food, 1 = going to table
    private float cookTimer;

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
            if(!this.moveHelper.hasPath()) {
                // cook food...
                this.cookTimer += Time.deltaTime;
                if(this.cookTimer >= cookSpeed) {
                    this.cookData.plateState = CellBehaviorTable.EnumPlateState.FULL;
                    this.stove.setOccupant(null);
                    this.stage = 1;
                    if(this.moveHelper.setDestination(this.table.pos, true) == null) {
                        // Can no longer go to table...
                        //TODO
                    }
                }
            }
        } else { // state == 1
            if(!this.moveHelper.hasPath()) {
                this.cookData.plateState = CellBehaviorTable.EnumPlateState.NONE;
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
        if(this.cookData.plateState == CellBehaviorTable.EnumPlateState.CLEAN) {
            foreach(CellBehaviorTable table in this.owner.world.getAllBehaviors<CellBehaviorTable>()) {
                if(table.plateState == CellBehaviorTable.EnumPlateState.NONE && table.isOccupantSitting()) {
                    // Someone needs food.

                    // Make sure there is both a fridge and stove that the cook can get to
                    bool foundStove = false;
                    bool foundFridge = false;

                    // Make use there is a stove, and go to it
                    foreach(CellBehaviorStove stove in this.owner.world.getAllBehaviors<CellBehaviorStove>()) {
                        if(this.moveHelper.setDestination(stove.pos + Rotation.DOWN, false) != null) {
                            this.moveHelper.setPathEndingRotation(Rotation.UP);

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

    /*
    private void find<T>(out CellBehavior behaviorRef) where T : CellBehavior {
        foreach(T stove in this.owner.world.getAllBehaviors<T>()) {
            if(this.moveHelper.setDestination(stove.pos + Rotation.DOWN, false) != null) {
                foundStove = true;

                this.stove = stove;
                this.stove.setOccupant(this.owner);

                this.table = table;
                this.stage = 0;

                return true;
            }
        }
    }
    */
}
