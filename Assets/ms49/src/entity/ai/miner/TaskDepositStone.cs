using UnityEngine;

public class TaskDepositStone : TaskBase<EntityWorker> {

    [SerializeField]
    private MinerMetaData minerData = null;

    private CellBehaviorDepositPoint depositPoint;

    public override bool shouldExecute() {
        if(this.minerData.heldItem != null) {
            this.gotoClosestBehavior<CellBehaviorDepositPoint>(
                ref this.depositPoint,
                true,
                b => b.isMaster || !b.isFull);
            if(this.depositPoint != null) {
                return true;
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.minerData.heldItem != null && this.depositPoint != null) {
            return true;
        }

        return false;
    }

    public override void preform() {
        if(!this.moveHelper.hasPath()) {
            if(!this.depositPoint.isFull) {
                this.depositPoint.deposit(this.minerData.heldItem);
                this.minerData.heldItem = null;
            }
        }
    }

    public override void resetTask() {
        base.resetTask();

        this.depositPoint = null;
    }
}
