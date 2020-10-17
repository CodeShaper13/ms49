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
            bool flag =
                this.minerData.heldItem != null &&
                this.depositPoint != null &&// Deposit Point hasn't be destroyed
                this.depositPoint.isOpen();

            if(!flag) {
                print("stopping execution 1:");
                print(this.minerData.heldItem != null);
                print(this.depositPoint != null); // Deposit Point hasn't be destroyed
                print(this.depositPoint.isOpen());
            }

            return flag;
        } else {
            bool flag =
                this.minerData.heldItem == null && // Miner isn't holding anything
                this.unloadPoint != null && // Unload Point hasn't been destroyed
                this.unloadPoint.minecart != null && // Unload Point still has a cart
                !this.unloadPoint.minecart.inventory.isEmpty(); // The cart isn't empty.

            if(!flag) {
                print("stop ecexution 2:");
                print(this.minerData.heldItem == null); // Miner isn't holding anything
                print(this.unloadPoint != null); // Unload Point hasn't been destroyed
                print(this.unloadPoint.minecart != null); // Unload Point still has a cart
                print(!this.unloadPoint.minecart.inventory.isEmpty());
            }

            return flag;
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

        print(this.owner.name + " is stopping the unloading");

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
                        !behavior.minecart.inventory.isEmpty() &&
                        (behavior == this.unloadPoint || !behavior.isOccupied());
                }) != null) {
            this.isDeliveringItem = false;
            this.unloadPoint.setOccupant(this.owner);
            return true;
        }

        return false;
    }
}
