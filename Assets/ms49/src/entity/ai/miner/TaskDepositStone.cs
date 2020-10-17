using UnityEngine;

public class TaskDepositStone : TaskBase<EntityWorker> {

    [SerializeField]
    private MinerMetaData minerData = null;

    private AbstractDepositPoint depositPoint;

    public override bool shouldExecute() {
        if(this.minerData.heldItem != null) {
            this.gotoClosestBehavior(
                ref this.depositPoint,
                true,
                b => b.isOpen());
            if(this.depositPoint != null) {
                return true;
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.minerData.heldItem != null && this.depositPoint != null && this.depositPoint.isOpen()) {
            return true;
        }

        return false;
    }

    public override void preform() {
        if(!this.moveHelper.hasPath()) {
            if(this.depositPoint.isOpen()) {
                this.depositPoint.deposit(this.minerData.heldItem);
                this.minerData.heldItem = null;
            }
        }
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.depositPoint = null;
    }
}
