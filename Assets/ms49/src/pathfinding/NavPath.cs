using UnityEngine;

public class NavPath {

    public int targetIndex;

    private PathPoint[] points;

    public NavPath(PathPoint[] points) {
        this.points = points;
    }

    public int getPointCount() {
        return this.points.Length;
    }

    public Vector2 getPoint(int index) {
        return this.points[index].worldPos;
    }
}
