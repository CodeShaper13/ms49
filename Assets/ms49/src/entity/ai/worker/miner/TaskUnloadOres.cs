using UnityEngine;

public class TaskUnloadOres : TaskMovement<EntityWorker> {

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
                this.depositPoint != null &&// Deposit Point hasn't be destroyed
                this.depositPoint.isOpen();
        } else {
            return
                this.minerData.heldItem == null && // Miner isn't holding anything
                this.unloadPoint != null && // Unload Point hasn't been destroyed
                this.unloadPoint.minecart != null && // Unload Point still has a cart
                !this.unloadPoint.minecart.inventory.isEmpty(); // The cart isn't empty.
        }
    }

    public override void onAtDestination() {
        base.onAtDestination();

        if(this.isDeliveringItem) {
            // Drop off item at deposit point.
            this.depositPoint.deposit(this.minerData.heldItem);
            this.minerData.heldItem = null;

            this.locateUnloadPoint();
        }
        else {
            // Pick up item.
            this.minerData.heldItem = this.unloadPoint.minecart.inventory.pullItem();

            this.isDeliveringItem = true;

            // Head to the deposit point.
            this.depositPoint = this.calculateAndSetPathToClosest<AbstractDepositPoint>(
                true,
                behavior => behavior.isOpen());

            // Manualy set it
            this.agent.setPath(this.navPath);
        }
    }

    public override void preform() {
        base.preform();

        if(this.owner.moveHelper.path != this.navPath) {
            this.owner.moveHelper.setPath(this.navPath);
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
        this.unloadPoint = this.calculateAndSetPathToClosest<CellBehaviorRailUnloadPoint>(
            true,
            behavior =>
                    behavior.minecart != null &&
                    !behavior.minecart.inventory.isEmpty() &&
                    (behavior == this.unloadPoint || !behavior.isOccupied()));

        if(this.unloadPoint != null) {
            this.isDeliveringItem = false;
            this.unloadPoint.setOccupant(this.owner);

            return true;
        }

        return false;
    }
}
