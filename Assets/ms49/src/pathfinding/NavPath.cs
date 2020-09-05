/// <summary>
/// Holds the Data of a path that a 
/// </summary>
public class NavPath {

    private PathPoint[] _points;

    /// <summary> If null, the Worker's rotation is not changed. </summary>
    public Rotation endingLookDirection { get; set; }
    public int targetIndex { get; private set; }

    public int pointCount => this._points.Length;
    public PathPoint firstPoint => this._points[0];
    public PathPoint targetPoint => this._points[this.targetIndex];
    public PathPoint endPoint => this._points[this.pointCount - 1];

    public NavPath(PathPoint[] points, Rotation endRot) {
        this._points = points;
        this.endingLookDirection = endRot;
        this.targetIndex = 0;
    }

    public PathPoint getPoint(int index) {
        return this._points[index];
    }

    public void nextPoint() {
        if(this.targetIndex < this.pointCount) {
            this.targetIndex++;
        }
    }
}
