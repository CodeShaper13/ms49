public class TaskSleep : TaskEatSleepBase<CellBehaviorBed> {

    public TaskSleep(EntityWorker owner, MoveHelper moveHelper) : base(owner, moveHelper, 1, 20, 99) { }

    public override void resetTask() {
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

            this.owner.setSleeping(false);
        }

        base.resetTask();
    }

    protected override void onStartRecharge() {
        base.onStartRecharge();

        this.owner.setSleeping(true);

        this.owner.faces.setFace(WorkerFaces.EnumFace.SLEEPING);
    }

    protected override float getStat() {
        return this.owner.getEnergy();
    }

    protected override void setStat(float value) {
        this.owner.setEnergy(value);
    }
}
