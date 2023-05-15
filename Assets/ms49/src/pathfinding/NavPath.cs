/// <summary>
/// Holds the Data of a path that a 
/// </summary>
public class NavPath {

    private Position[] points;

    /// <summary> If null, the Agent's rotation is not changed. </summary>
    public Rotation endingLookDirection { get; set; }
    public int targetIndex { get; private set; }

    public int PointCount => this.points.Length;
    public Position FirstPoint => this.points[0];
    public Position TargetPoint => this.points[this.targetIndex];
    public Position EndPoint => this.points[this.PointCount - 1];
    public Position this[int index] => this.points[index];

    public NavPath(Position[] points, Rotation endRot) {
        this.points = points;
        this.endingLookDirection = endRot;
        this.targetIndex = 0;
    }

    public void NextPoint() {
        if(this.targetIndex < this.PointCount) {
            this.targetIndex++;
        }
    }
}
