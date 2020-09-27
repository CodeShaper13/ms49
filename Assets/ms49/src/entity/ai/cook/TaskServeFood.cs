using UnityEngine;

public class TaskServeFood : TaskBase<EntityWorker> {

    [SerializeField]
    private CookMetaData cookData = null;

    private CellBehaviorTable table;

    public override bool continueExecuting() {
        return
            this.cookData.plateState == CellBehaviorTable.EnumPlateState.FULL &&
            this.table != null &&
            this.table.isOccupied();
    }

    public override void preform() {
        if(!this.moveHelper.hasPath()) {
            this.table.plateState = CellBehaviorTable.EnumPlateState.FULL;

            this.cookData.plateState = CellBehaviorTable.EnumPlateState.NONE;
        }
    }

    public override bool shouldExecute() {
        if(this.cookData.plateState == CellBehaviorTable.EnumPlateState.FULL) {
            foreach(CellBehaviorTable table in this.owner.world.getAllBehaviors<CellBehaviorTable>()) {
                if(table.plateState == CellBehaviorTable.EnumPlateState.NONE && table.isOccupantSitting()) {

                    if(this.moveHelper.setDestination(table.pos, true) != null) {
                        this.table = table;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
