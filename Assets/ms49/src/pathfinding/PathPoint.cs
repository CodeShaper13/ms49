using UnityEngine;

public struct PathPoint {

    private int x;
    private int y;
    public int depth { get; private set; }

    public Vector2Int cellPos => new Vector2Int(this.x, this.y);
    public Vector2 worldPos => new Vector2(this.x + 0.5f, this.y + 0.5f);
    public Position position => new Position(this.x, this.y, this.depth);

    public PathPoint(int x, int y, int depth) {
        this.x = x;
        this.y = y;
        this.depth = depth;
    }

    public PathPoint(Position pos) {
        this.x = pos.x;
        this.y = pos.y;
        this.depth = pos.depth;
    }
}
