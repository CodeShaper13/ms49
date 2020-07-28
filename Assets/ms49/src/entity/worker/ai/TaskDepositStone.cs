using UnityEngine;
using System.Collections;

public class TaskDepositStone : TaskBase<EntityMiner> {

    private CellBehaviorDepositPoint depositPoint;

    public TaskDepositStone(EntityMiner owner, MoveHelper moveHelper) : base(owner, moveHelper) { }
    
    public override bool shouldExecute() {
        if(this.owner.heldItem != null) {
            CellBehaviorDepositPoint point = this.findClosestDepositArea();
            if(point != null) {
                this.depositPoint = point;

                this.moveHelper.setDestination(this.depositPoint.pos, true);

                return true;
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.owner.heldItem != null && this.depositPoint != null) {
            // TODO make sure deposit point still exists.
            return true;
        }

        return false;
    }

    public override void preform() {
        if(Vector2.Distance(this.owner.getCellPos(), this.depositPoint.pos.vec2) <= 1f) {
            this.owner.heldItem = this.depositPoint.deposit(this.owner.heldItem);
        }
    }

    public override void resetTask() {
        base.resetTask();

        this.depositPoint = null;
    }


    /// <summary>
    /// Returns the closest depoisit point to the Worker.
    /// </summary>
    private CellBehaviorDepositPoint findClosestDepositArea() {
        //TODO make sure it can be reached
        CellBehaviorDepositPoint point = this.owner.world.getClosestBehavior<CellBehaviorDepositPoint>(this.owner.position);
        return point;
    }
}
