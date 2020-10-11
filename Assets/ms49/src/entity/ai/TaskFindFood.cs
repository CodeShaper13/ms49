using UnityEngine;

public class TaskFindFood : TaskBase<EntityWorker> {

    [SerializeField]
    private float stopEattingValue = 99;
    [SerializeField]
    private float eatSpeed = 20;
    [SerializeField]
    private float _startFoodHuntAt = 20;

    public float startFoodHuntAt => this._startFoodHuntAt;

    protected CellBehaviorTable table;

    public override bool continueExecuting() {
        return this.owner.hunger.value < stopEattingValue;
    }

    public override void preform() {
        if(this.table.plateState == CellBehaviorTable.EnumPlateState.FULL && this.owner.position.distance(table.chairPos) <= 1) {

            this.owner.hunger.increase(this.eatSpeed * Time.deltaTime);
            if(this.owner.hunger.value >= this.stopEattingValue) {
                this.table.plateState = CellBehaviorTable.EnumPlateState.DIRTY;
            }

            //this.owner.animator.playClip("Eating"); // TODO remake animation
        }
    }

    public override bool shouldExecute() {
        if(this.owner.hunger.value <= startFoodHuntAt) {
            // Find a table.
            foreach(CellBehaviorTable table in this.owner.world.getAllBehaviors<CellBehaviorTable>()) {
                if(table.hasChair && !table.isOccupied()) {
                    if(this.moveHelper.setDestination(table.chairPos) != null) {
                        this.moveHelper.setPathEndingRotation(Rotation.directionToRotation(table.center - table.chair.behavior.center));
                        this.table = table;
                        this.table.setOccupant(this.owner); // Reserve it, so no one takes it.
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override void onTaskStop() {
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

        base.onTaskStop();
    }
}
