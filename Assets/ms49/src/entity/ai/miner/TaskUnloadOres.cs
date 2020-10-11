using UnityEngine;

public class TaskUnloadOres : TaskBase<EntityWorker> {

    [SerializeField]
    private MinerMetaData minerData = null;

    private CellBehaviorRailUnloadPoint unloadPoint;
    private AbstractDepositPoint depositPoint;
    private bool isDeliveringItem = false;

    public override bool shouldExecute() {
        if(this.minerData.heldItem == null) {
            return this.locateUnloadPoint();
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.isDeliveringItem) {
            return
                this.minerData.heldItem != null &&
                this.depositPoint != null;
        } else {
            return
                this.minerData.heldItem == null &&
                this.unloadPoint != null &&
                this.unloadPoint.minecart != null &&
                !this.unloadPoint.minecart.inventory.isEmpty();
        }
    }

    public override void preform() {
        if(!this.moveHelper.hasPath()) {
            if(this.isDeliveringItem) {
                // Drop off item at deposit point.
                this.depositPoint.deposit(this.minerData.heldItem);
                this.minerData.heldItem = null;

                this.locateUnloadPoint();
            }
            else {
                // Pick up item.
                this.minerData.heldItem = this.unloadPoint.minecart.inventory.pullItem();

                // Head to the deposit point.
                if(this.gotoClosestBehavior<AbstractDepositPoint>(
                    ref this.depositPoint,
                    true,
                    (behavior) => {
                        return behavior.isOpen();
                    }) != null) {

                    this.isDeliveringItem = true;
                }
            }
        }
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.isDeliveringItem = false;

        if(this.unloadPoint != null) {
            this.unloadPoint.setOccupant(null);
        }

        this.unloadPoint = null;
        this.depositPoint = null;
    }

    private bool locateUnloadPoint() {
        if(this.gotoClosestBehavior<CellBehaviorRailUnloadPoint>(
                ref this.unloadPoint,
                true,
                (behavior) => {
                    return
                        behavior.minecart != null &&
                        !behavior.isOccupied() &&
                        !behavior.minecart.inventory.isEmpty();
                }) != null) {
            this.isDeliveringItem = false;
            this.unloadPoint.setOccupant(this.owner);
            return true;
        }

        return false;
    }
}
