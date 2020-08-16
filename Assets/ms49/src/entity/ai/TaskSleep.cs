using UnityEngine;

public class TaskSleep : TaskBase<EntityWorker> {

    protected CellBehaviorOccupiable occupiable;
    protected bool rechargingAtSpot;

    private readonly float rechargeSpeed = 1;
    private readonly float valueToStartHuntAt = 20;
    private readonly float valueToStop = 99;

    public TaskSleep(EntityWorker owner, MoveHelper moveHelper) : base(owner, moveHelper) { }

    public override bool continueExecuting() {
        return this.owner.energy < this.valueToStop;
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
            this.owner.setEnergy(this.owner.energy + (this.rechargeSpeed * Time.deltaTime));
        }
    }

    public override bool shouldExecute() {
        if(this.owner.energy > this.valueToStartHuntAt) {
            return false; // They don't need food/sleep yet.
        } else {
            // Find a structure
            CellBehaviorBed behavior = this.owner.world.getClosestBehavior<CellBehaviorBed>(this.owner.position, b => !b.isOccupied());
            if(behavior != null) {
                if(this.moveHelper.setDestination(behavior.pos) != null) {
                    this.occupiable = behavior;
                    this.occupiable.setOccupant(this.owner); // Reserve it, so no one takes it.
                    return true;
                }
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
