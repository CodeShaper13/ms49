public class PathfindingNode : IHeapItem<PathfindingNode> {

    public readonly int x;
    public readonly int y;
    public readonly int depth;
    public readonly EnumZMoveDirection zMoveDir;
    public readonly int movementPenalty;

    public int gCost;
    public int hCost;
    public PathfindingNode parent;

    private int heapIndex;

    public bool IsWalkable => this.movementPenalty >= 0;
    public Position Position => new Position(this.x, this.y, this.depth);
    public int FCost => this.gCost + this.hCost;
    public bool ConnectsUp =>
        this.zMoveDir == EnumZMoveDirection.Up ||
        this.zMoveDir == EnumZMoveDirection.Both;
    public bool ConnectsDown =>
        this.zMoveDir == EnumZMoveDirection.Down ||
        this.zMoveDir == EnumZMoveDirection.Both;
    // Required from IHeapItem
    public int HeapIndex {
        get => this.heapIndex;
        set {
            this.heapIndex = value;
        }
    }

    public PathfindingNode(int x, int y, int depth, EnumZMoveDirection zMoveDir, int penalty) {
        this.x = x;
        this.y = y;
        this.depth = depth;
        this.zMoveDir = zMoveDir;
        this.movementPenalty = penalty;
    }

    public override string ToString() {
        return string.Format("Node([{0}, {1}], IsWalkable:{2})", this.x, this.y, this.IsWalkable);
    }

    public int CompareTo(PathfindingNode nodeToCompare) {
        int compare = this.FCost.CompareTo(nodeToCompare.FCost);
        if(compare == 0) {
            compare = this.hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}