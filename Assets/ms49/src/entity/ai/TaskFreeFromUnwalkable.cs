
/// <summary>
/// Workers who are standing on an unwalkable cell will try and move to an adjacent free one.
/// </summary>
public class TaskFreeFromUnwalkable : TaskBase<EntityWorker> {

    private Position targetPos;

    public override bool continueExecuting() {
        if(!this.moveHelper.hasPath()) {
            return false;
        }

        return true;
    }

    public override void preform() { }

    public override bool shouldExecute() {
        if(!this.owner.world.isOutOfBounds(this.owner.position)) {
            if(this.owner.world.getCellState(this.owner.position).data.isWalkable) {

                // Check if there is an adjacent cell that is free
                foreach(Rotation r in Rotation.ALL) {
                    Position p1 = this.owner.position + r;
                    if(!this.owner.world.isOutOfBounds(p1) && this.owner.world.getCellState(p1).data.isWalkable) {
                        this.moveHelper.setDestination(p1);
                        this.targetPos = p1;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
