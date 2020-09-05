using UnityEngine;

public class TaskSleep : TaskBase<EntityWorker> {

    protected CellBehaviorBed occupiable;
    protected bool rechargingAtSpot;

    [SerializeField]
    private float energyRechargeSpeed = 1;
    [SerializeField]
    private float seekBedAt = 20;
    [SerializeField]
    private float valueToStop = 99;

    public override bool continueExecuting() {
        return this.owner.energy.value < this.valueToStop;
    }

    public override void preform() {
        if(!this.moveHelper.hasPath()) {
            if(!this.rechargingAtSpot) {
                occupiable.setOccupant(this.owner);
                this.rechargingAtSpot = true;

                this.owner.setSleeping(true);
            }

            this.owner.animator.playClip("Sleeping");
        }

        if(this.rechargingAtSpot) {
            this.owner.energy.value = (this.owner.energy.value + (this.energyRechargeSpeed * Time.deltaTime));
        }
    }

    public override bool shouldExecute() {
        if(this.owner.energy.value > this.seekBedAt) {
            return false; // They don't need food/sleep yet.
        } else {
            // Find a structure
            this.gotoClosestBehavior<CellBehaviorBed>(
                ref this.occupiable,
                false,
                b => !b.isOccupied());
            if(this.occupiable != null) {
                this.occupiable.setOccupant(this.owner); // Reserve it, so no one takes it.
                return true;
            }

            return false;
        }
    }

    public override void resetTask() {
        base.resetTask();

        if(this.occupiable != null) {
            // Try to move away from the bed so it "looks free".
            // If there no walkable space, stay on the bed.  Others will still be able to claim it.
            Position? freeSpot = this.getFreeSpot(this.owner.position);
            if(freeSpot == null) {
                freeSpot = this.getFreeSpot(this.owner.position + Rotation.UP); // Head of the bed.
            }

            if(freeSpot != null) {
                this.moveHelper.setDestination(
                    (Position)freeSpot,
                    false);
            }

            this.occupiable.setOccupant(null);

            this.owner.setSleeping(false);
        }

        this.occupiable = null;
        this.rechargingAtSpot = false;
    }
}
