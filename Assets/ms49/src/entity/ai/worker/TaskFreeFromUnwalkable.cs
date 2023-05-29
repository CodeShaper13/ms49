
/// <summary>
/// Workers who are standing on an unwalkable cell will try and move to an adjacent free one.
/// </summary>
public class TaskFreeFromUnwalkable : TaskMovement<EntityWorker> {

    private Position targetPos;

    public override bool shouldExecute() {
        if(!this.owner.world.IsOutOfBounds(this.owner.Position)) {
            if(this.owner.world.GetCellState(this.owner.Position).data.IsWalkable) {

                // Check if there is an adjacent cell that is free
                foreach(Rotation r in Rotation.ALL) {
                    Position p1 = this.owner.Position + r;
                    if(!this.owner.world.IsOutOfBounds(p1) && this.owner.world.GetCellState(p1).data.IsWalkable) {
                        this.navPath = this.agent.CalculatePath(p1);
                        if(navPath != null) {
                            return true;
                        }

                        this.targetPos = p1;
                    }
                }
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.owner.WorldPos != this.navPath.EndPoint.Center) {
            return true;
        }

        return false;
    }
}
