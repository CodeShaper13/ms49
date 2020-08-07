using UnityEngine;


/// <summary>
/// Executes if the owner has a dirt plate in hand.
/// 
/// This will make them travel to a sink and wash the plate.
/// </summary>
public class TaskWashPlate : TaskBase<EntityCook> {

    private const float WASH_TIME = 4f;

    private float washTimer;
    private CellBehaviorSink sink;

    public TaskWashPlate(EntityCook owner, MoveHelper moveHelper) : base(owner, moveHelper) { }

    public override bool continueExecuting() {
        if(this.sink == null || this.owner.plateState != CellBehaviorTable.EnumPlateState.DIRTY) {
            return false;
        } else {
            return true;
        }
    }

    public override void preform() {
        if(this.owner.position.distance(this.sink.pos) <= 1f) {
            this.sink.setFilled(true);

            this.washTimer += Time.deltaTime;
            if(this.washTimer >= WASH_TIME) {
                this.owner.plateState = CellBehaviorTable.EnumPlateState.CLEAN;
                this.sink.setOccupant(null);
                this.sink.setFilled(false);
            }
        }
    }

    public override void resetTask() {
        base.resetTask();

        this.washTimer = 0f;
        if(this.sink != null) {
            this.sink.setOccupant(null);
            this.sink.setFilled(false);
        }
    }

    public override bool shouldExecute() {
        if(this.owner.plateState == CellBehaviorTable.EnumPlateState.DIRTY) {
            foreach(CellBehaviorSink sink in this.owner.world.getAllBehaviors<CellBehaviorSink>()) {
                if(!sink.isOccupied()) {
                    if(this.moveHelper.setDestination(sink.pos, true) != null) {
                        this.sink = sink;
                        this.sink.setOccupant(this.owner);
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
