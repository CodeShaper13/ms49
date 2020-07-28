using UnityEngine;

public struct PathPoint {

    public float x;
    public float y;
    public int depth;

    public PathPoint(float x, float y, int depth) {
        this.x = x;
        this.y = y;
        this.depth = depth;
    }

    public Vector2 worldPos {
        get { return new Vector2(this.x, this.y);  }
    }
}
