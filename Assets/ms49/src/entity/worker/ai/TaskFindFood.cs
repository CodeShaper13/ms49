using UnityEngine;

public class TaskFindFood : TaskEatSleepBase<CellBehaviorChair> {

    public TaskFindFood(EntityWorker owner, MoveHelper moveHelper) : base(owner, moveHelper, 20, 20, 99) { }

    public override void resetTask() {
        if(this.occupiable != null) {
            Debug.Log("resetting");

            // Try to move away from the chair so it "looks free".
            // If there no walkable space, stay on the chair.  Others will still be able to claim it.
            Position? freeSpot = this.getFreeSpot(this.owner.position);
            if(freeSpot != null) {
                this.moveHelper.setDestination(
                    (Position)freeSpot,
                    false);
            }
        }

        base.resetTask();
    }

    protected override float getStat() {
        return this.owner.getHunger();
    }

    protected override void setStat(float value) {
        this.owner.setHunger(value);
    }

    protected override void onRecharge() {
        base.onRecharge();

        this.owner.faces.setFace(((int)Time.time) % 2 == 0 ? WorkerFaces.EnumFace.EATING_1 : WorkerFaces.EnumFace.EATING_2);
    }
}
