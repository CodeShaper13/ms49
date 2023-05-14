using UnityEngine;

public class PathfindingNode : IHeapItem<PathfindingNode> {

    public readonly int x;
    public readonly int y;
    public readonly int depth;
    public readonly EnumZMoveDirection zMoveDir;
    public readonly int movementPenalty;

    public int gCost;
    public int hCost;
    public PathfindingNode parent;

    public Vector2 worldPosition;

    int heapIndex;

    public PathfindingNode(Vector2 worldPos, int x, int y, int depth, EnumZMoveDirection zMoveDir, int penalty) {
        this.worldPosition = worldPos;
        this.x = x;
        this.y = y;
        this.depth = depth;
        this.zMoveDir = zMoveDir;
        this.movementPenalty = penalty;
    }

    public PathPoint asPathPoint() {
        return new PathPoint(this.x, this.y, this.depth);
    }

    public bool connectsUp() {
        return this.zMoveDir == EnumZMoveDirection.Up ||
            this.zMoveDir == EnumZMoveDirection.Both;
    }

    public bool connectsDown() {
        return this.zMoveDir == EnumZMoveDirection.Down ||
            this.zMoveDir == EnumZMoveDirection.Both;
    }

    public bool isWalkable {
        get {
            return this.movementPenalty >= 0;
        }
    }

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public override string ToString() {
        return "Node(" + this.x + "," + this.y + ")(walkable:" + this.isWalkable.ToString() + ")";
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(PathfindingNode nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}