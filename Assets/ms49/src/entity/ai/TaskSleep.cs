using UnityEngine;

public class TaskSleep : TaskBase<EntityWorker> {

    protected CellBehaviorBed bed;
    protected bool layingInBed;

    [SerializeField]
    private float energyRechargeSpeed = 1;
    [SerializeField]
    private float _seekBedAt = 30;
    [SerializeField]
    private float valueToStop = 99;

    public float seekBedAt => this._seekBedAt;

    public override bool continueExecuting() {
        return this.owner.energy.value < this.valueToStop;
    }

    public override void preform() {
        if(!this.moveHelper.hasPath()) {
            if(!this.layingInBed) {
                this.bed.setOccupant(this.owner);
                this.layingInBed = true;

                this.owner.setSleeping(true);
            }

            this.owner.animator.playClip("Sleeping");

            if(this.layingInBed) {
                this.owner.energy.value = (this.owner.energy.value + (this.energyRechargeSpeed * Time.deltaTime));
            }
        }        
    }

    public override bool shouldExecute() {
        if(this.owner.energy.value <= this.seekBedAt) {
            // Find a bed.
            this.gotoClosestBehavior<CellBehaviorBed>(
                ref this.bed,
                false,
                behavior => !behavior.isOccupied());

            if(this.bed != null) {
                this.bed.setOccupant(this.owner); // Reserve it, so no one takes it.
                return true;
            }            
        }

        return false;
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
                this.moveHelper.setDestination(
                    (Position)freeSpot,
                    false);
            }

            this.bed.setOccupant(null);

            this.owner.setSleeping(false);
        }

        this.bed = null;
        this.layingInBed = false;
    }
}
