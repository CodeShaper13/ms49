using UnityEngine;

public class Node : IHeapItem<Node> {

    public readonly bool isWalkable;
    public readonly int x;
    public readonly int y;
    public readonly int depth;
    public EnumZMoveDirection zMoveDir;

    public int gCost;
    public int hCost;
    public Node parent;

    public Vector2 worldPosition;

    int heapIndex;

    public Node(bool walkable, Vector2 worldPos, int x, int y, int depth, EnumZMoveDirection zMoveDir) {
        this.isWalkable = walkable;
        this.worldPosition = worldPos;
        this.x = x;
        this.y = y;
        this.depth = depth;
        this.zMoveDir = zMoveDir;
    }

    public PathPoint asPathPoint() {
        return new PathPoint(this.worldPosition.x, this.worldPosition.y, this.depth);
    }

    public bool connectsUp() {
        return this.zMoveDir == EnumZMoveDirection.UP ||
            this.zMoveDir == EnumZMoveDirection.BOTH;
    }

    public bool connectsDown() {
        return this.zMoveDir == EnumZMoveDirection.DOWN ||
            this.zMoveDir == EnumZMoveDirection.BOTH;
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

    public int CompareTo(Node nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}