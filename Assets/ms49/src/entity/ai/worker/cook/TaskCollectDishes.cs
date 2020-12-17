using UnityEngine;
/// <summary>
/// Executes if the owner has no plate in hand and there is
/// a table with a dirty plate.
/// 
/// This will make the owner travel to a table with a dirty
/// plate and pick it up.
/// </summary>
public class TaskCollectDishes : TaskMovement<EntityWorker> {

    [SerializeField]
    private CookMetaData cookData = null;

    private CellBehaviorTable table;

    public override bool shouldExecute() {
        if(this.cookData.plateState == CellBehaviorTable.EnumPlateState.NONE || this.cookData.plateState == CellBehaviorTable.EnumPlateState.CLEAN) {
            this.table = this.calculateAndSetPathToClosest<CellBehaviorTable>(
                true,
                behavior => behavior.plateState == CellBehaviorTable.EnumPlateState.DIRTY);

            if(this.table != null) {
                return true;
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.table == null || this.table.plateState != CellBehaviorTable.EnumPlateState.DIRTY) {
            return false;
        } else {
            return true;
        }
    }

    public override void onAtDestination() {
        base.onAtDestination();

        this.cookData.plateState = CellBehaviorTable.EnumPlateState.DIRTY;
        this.table.plateState = CellBehaviorTable.EnumPlateState.NONE;
    }
}
