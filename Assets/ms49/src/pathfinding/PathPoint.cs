using UnityEngine;

public struct PathPoint {

    private int x;
    private int y;
    public int depth { get; private set; }

    public Vector2Int cellPos => new Vector2Int(this.x, this.y);
    public Vector2 worldPos => new Vector2(this.x + 0.5f, this.y + 0.5f);

    public PathPoint(int x, int y, int depth) {
        this.x = x;
        this.y = y;
        this.depth = depth;
    }
}
