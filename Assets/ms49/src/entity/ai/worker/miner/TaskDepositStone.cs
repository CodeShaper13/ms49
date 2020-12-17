using UnityEngine;

public class TaskDepositStone : TaskMovement<EntityWorker> {

    [SerializeField]
    private MinerMetaData minerData = null;

    private AbstractDepositPoint depositPoint;

    public override bool shouldExecute() {
        if(this.minerData.heldItem != null) {
            this.depositPoint = this.calculateAndSetPathToClosest<AbstractDepositPoint>(
                true,
                behavior => behavior.isOpen());

            if(this.navPath != null) {
                return this.navPath != null;
            } else {
                this.owner.emote.startEmote(new Emote("question", 0.1f).setTooltip("Can't find a deposit point"));
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

    public override void onTaskStop() {
        base.onTaskStop();

        this.depositPoint = null;
    }

    public override void onDestinationArive() {
        if(this.depositPoint is CellBehaviorMasterDepositPoint) {
            if(Random.value < this.owner.info.personality.stealPercent) {
                // Take the item
                this.minerData.heldItem = null;
                return;
            }
        }

        this.depositPoint.deposit(this.minerData.heldItem);
        this.minerData.heldItem = null;
    }
}
