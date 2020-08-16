using UnityEngine;

public class TaskFindFood : TaskBase<EntityWorker> {

    private const float EAT_STOP_AT = 99;
    private const float EAT_SPEED = 20;
    private const float START_HUNT_HUNGER = 20;

    protected CellBehaviorTable table;

    public TaskFindFood(EntityWorker owner, MoveHelper moveHelper) : base(owner, moveHelper) { }

    public override bool continueExecuting() {
        return this.owner.hunger < EAT_STOP_AT;
    }

    public override void preform() {
        if(this.table.plateState == CellBehaviorTable.EnumPlateState.FULL && this.owner.position.distance(table.chairPos) <= 1) {
            this.owner.hunger += EAT_SPEED * Time.deltaTime;
            //this.owner.faces.setFace(((int)Time.time) % 2 == 0 ? WorkerFaces.EnumFace.EATING_1 : WorkerFaces.EnumFace.EATING_2);
            if(this.owner.hunger >= EAT_STOP_AT) {
                this.table.plateState = CellBehaviorTable.EnumPlateState.DIRTY;
            }

            this.owner.animator.playClip("Eating");
        }
    }

    public override bool shouldExecute() {
        if(this.owner.hunger <= START_HUNT_HUNGER) {
            // Find a table.
            foreach(CellBehaviorTable table in this.owner.world.getAllBehaviors<CellBehaviorTable>()) {
                if(table.hasChair && !table.isOccupied()) {
                    if(this.moveHelper.setDestination(table.chairPos) != null) {
                        this.table = table;
                        this.table.setOccupant(this.owner); // Reserve it, so no one takes it.
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override void resetTask() {
        if(this.table != null) {
            this.table.setOccupant(null);

            // Try to move away from the chair so it "looks free".
            // If there no walkable space, stay on the chair.  Others will still be able to claim it.
            Position? freeSpot = this.getFreeSpot(this.owner.position);
            if(freeSpot != null) {
                this.moveHelper.setDestination(
                    (Position)freeSpot,
                    false);
            }

            this.owner.animator.stopClip();
        }

        base.resetTask();
    }
}
