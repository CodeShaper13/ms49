public class TaskDepositStone : TaskBase<EntityMiner> {

    private CellBehaviorDepositPoint depositPoint;
    
    public override bool shouldExecute() {
        if(this.owner.heldItem != null) {
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
        if(this.owner.heldItem != null && this.depositPoint != null) {
            return true;
        }

        return false;
    }

    public override void preform() {
        if(!this.moveHelper.hasPath()) {
            if(!this.depositPoint.isFull) {
                this.depositPoint.deposit(this.owner.heldItem);
                this.owner.heldItem = null;
            }
        }
    }

    public override void resetTask() {
        base.resetTask();

        this.depositPoint = null;
    }
}
