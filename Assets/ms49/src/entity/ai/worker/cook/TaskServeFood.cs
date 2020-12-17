using UnityEngine;

/// <summary>
/// Executes if the Cook has a full plate and there is a Worker who
/// is waiting at a table without a plate OR the Cook is hungry 
/// 
/// Causes the Cook to deliver the plate to the Worker.
/// </summary>
public class TaskServeFood : TaskMovement<EntityWorker> {

    [SerializeField]
    private CookMetaData cookData = null;

    private CellBehaviorTable table;

    public override bool shouldExecute() {
        if(this.cookData.plateState == CellBehaviorTable.EnumPlateState.FULL) {
            this.table = this.calculateAndSetPathToClosest<CellBehaviorTable>(
                true,
                behavior => behavior.plateState == CellBehaviorTable.EnumPlateState.NONE && behavior.isOccupantSitting());

            if(this.table != null) {
                return true;
            } else {
                this.owner.emote.startEmote(new Emote("exclamation", 0.1f).setTooltip("Can't find a way to the table"));
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        return
            this.cookData.plateState == CellBehaviorTable.EnumPlateState.FULL &&
            this.table != null &&
            this.table.isOccupied();
    }

    public override void onDestinationArive() {
        base.onDestinationArive();

        this.table.plateState = CellBehaviorTable.EnumPlateState.FULL;

        this.cookData.plateState = CellBehaviorTable.EnumPlateState.NONE;
    }
}
