using UnityEngine;

public class TaskSleep : TaskMovement<EntityWorker> {

    [SerializeField]
    private float energyRechargeSpeed = 1;
    [SerializeField]
    private float _seekBedAt = 30;

    private CellBehaviorBed bed;

    public int stopSleepingAt => (int)this.owner.energy.maxValue + this.owner.info.personality.sleepStopOffset;

    public override bool shouldExecute() {
        if(this.owner.energy.value <= this._seekBedAt + this.owner.info.personality.sleepStartOffset) {
            // Find a bed.
            this.bed = this.calculateAndSetPathToClosest<CellBehaviorBed>(
                false,
                behavior => !behavior.isOccupied());

            if(this.bed != null) {
                this.bed.setOccupant(this.owner); // Reserve it, so no one takes it.
                return true;
            } else {
                this.owner.emote.startEmote(new Emote("exclamation", 0.1f).setTooltip("Can't find a bed"));
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        return this.bed != null && this.owner.energy.value < this.stopSleepingAt;
    }

    public override void onTaskStart() {
        base.onTaskStart();

        this.owner.emote.startEmote(new Emote("bed", -1).setPriority());
    }

    public override void onDestinationArive() {
        base.onDestinationArive();

        this.bed.setOccupant(this.owner);

        this.owner.setSleeping(true);

        this.owner.emote.cancelEmote();
    }

    public override void onAtDestination() {
        base.onAtDestination();

        this.owner.animator.playClip("Sleeping");

        this.owner.energy.increase(this.energyRechargeSpeed * Time.deltaTime * this.owner.info.personality.energyGainMultiplyer);
    }

    public override void onTaskStop() {
        base.onTaskStop();

        if(this.bed != null) {
            // Try to move away from the bed so it "looks free".
            // If there no walkable space, stay on the bed.  Others will still be able to claim it.
            Position? freeSpot = this.getFreeSpot(this.owner.position);
            if(freeSpot == null) {
                freeSpot = this.getFreeSpot(this.owner.position + Rotation.UP); // Head of the bed.
            }

            if(freeSpot != null) {
                NavPath path =this.agent.CalculatePath(
                    (Position)freeSpot,
                    false);

                if(path != null) {
                    this.agent.SetPath(path);
                }
            }

            this.bed.setOccupant(null);
            this.owner.setSleeping(false);
        }

        this.bed = null;

        this.owner.emote.cancelEmote();
    }

    public override bool canBeInterupted() {
        return false;
    }
}
