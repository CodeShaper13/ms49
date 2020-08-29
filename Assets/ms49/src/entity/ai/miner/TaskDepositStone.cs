using UnityEngine;

public class TaskDepositStone : TaskBase<EntityMiner> {

    private CellBehaviorDepositPoint depositPoint;

    public TaskDepositStone(EntityMiner owner, MoveHelper moveHelper) : base(owner, moveHelper) { }
    
    public override bool shouldExecute() {
        if(this.owner.heldItem != null) {

            foreach(CellBehaviorDepositPoint point in this.owner.world.getAllBehaviors<CellBehaviorDepositPoint>()) {
                if(this.moveHelper.setDestination(point.pos, true) != null) {
                    this.depositPoint = point;
                    return true;
                }
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
            this.owner.heldItem = this.depositPoint.deposit(this.owner.heldItem);
        }
    }

    public override void resetTask() {
        base.resetTask();

        this.depositPoint = null;
    }
}
