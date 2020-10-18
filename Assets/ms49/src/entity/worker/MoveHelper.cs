using UnityEngine;

[RequireComponent(typeof(EntityWorker))]
public class MoveHelper : MonoBehaviour {

    [SerializeField, Min(0)]
    [Tooltip("How many units the Entity moves per second.")]
    private float speed = 1f;
    [SerializeField, Min(0)]
    [Tooltip("How long it takes to climb a ladder in seconds.")]
    private float _ladderClimbSpeed = 0.1f;
    
    private EntityWorker worker;
    private float layerChangeProgress;

    public NavPath path { get; private set; }

    private void Awake() {
        this.worker = this.GetComponent<EntityWorker>();
    }

    public void update() {
        if(this.hasPath()) {
            PathPoint currentWaypoint = this.path.targetPoint;
            Vector2 workerPos = this.worker.worldPos;

            bool posMatch = workerPos == currentWaypoint.worldPos;
            bool depthMatch = worker.depth == currentWaypoint.depth;

            if(posMatch) {
                if(depthMatch) {
                    this.path.nextPoint();
                    if(this.path.targetIndex >= path.pointCount) {
                        // Reached the end of the path.

                        if(this.path.endingLookDirection != null) {
                            this.worker.rotation = this.path.endingLookDirection;
                        }

                        this.stop();

                        return;
                    }
                    currentWaypoint = this.path.targetPoint;
                } else {
                    if(this.layerChangeProgress == 0) {
                        // First frame climbing, start an animation
                        if(this.worker.depth > currentWaypoint.depth) {
                            //this.worker.animator.playClip("ClimbLadderUp");
                        } else {
                            //this.worker.animator.playClip("ClimbLadderDown");
                        }
                    }

                    this.layerChangeProgress += Time.deltaTime;

                    if(this.layerChangeProgress > this._ladderClimbSpeed) {
                        this.layerChangeProgress = 0;
                        this.worker.depth = currentWaypoint.depth;
                    }
                }
            }

            // Move the Worker towards the next point on the X and Y axis.
            this.transform.position = Vector2.MoveTowards(
                transform.position,
                currentWaypoint.worldPos,
                this.speed * Time.deltaTime);

            // Update the direction the Worker is looking.
            Vector2 direction = workerPos - this.worker.posLastFrame;
            this.worker.rotation = Rotation.directionToRotation(direction);
        }
    }

    private void OnDrawGizmos() {
        if(this.hasPath()) {
            for(int i = this.path.targetIndex; i < this.path.pointCount; i++) {
                Gizmos.color = Color.yellow;

                if(i == this.path.targetIndex) {
                    Gizmos.DrawLine(
                        this.transform.position,
                        this.path.getPoint(i).worldPos);
                } else {
                    Gizmos.DrawLine(
                        this.path.getPoint(i - 1).worldPos,
                        this.path.getPoint(i).worldPos);
                }
            }

            for(int i = 0; i < this.path.pointCount; i++) {
                Gizmos.DrawSphere(this.path.getPoint(i).worldPos, 0.1f);
            }
        }
    }

    /// <summary>
    /// Stops the Worker.  This will delete the path.
    /// </summary>
    public void stop() {
        this.path = null;
    }

    /// <summary>
    /// Returns true if the Worker has a path that they are following.
    /// </summary>
    public bool hasPath() {
        return this.path != null;
    }

    /// <summary>
    /// Returns the distance from the Worker to the end of the path in a straight line.
    /// -1 is returned if the Worker has no path.
    /// </summary>
    public float getDirectDistanceToEnd() {
        if(!this.hasPath()) {
            return -1f;
        }

        float dis = Vector2.Distance(this.worker.getCellPos(), this.path.endPoint.worldPos);

        // Add the total number of levels that will be crossed
        dis += Mathf.Abs(this.path.endPoint.depth - this.worker.depth) * Position.LAYER_DISTANCE_COST;

        return dis;
    }

    /// <summary>
    /// Returns the distance the worker has left on their path.
    /// -1 is returned if the Worker has no path.
    /// </summary>
    public float getDistanceToEnd() {
        if(!this.hasPath()) {
            return -1f;
        }
        
        // Start off with the distance from the Worker to the next node.
        float dis = Vector2.Distance(this.worker.getCellPos(), this.path.targetPoint.worldPos);
        dis += Mathf.Abs(this.worker.depth - this.path.targetPoint.depth) * Position.LAYER_DISTANCE_COST;

        // Total the distances between the nodes.
        for(int i = this.path.targetIndex; i < this.path.pointCount - 1; i++) {
            PathPoint p1 = this.path.getPoint(i);
            PathPoint p2 = this.path.getPoint(i + 1);

            // X and Y distance.
            dis += Vector2.Distance(p1.worldPos, p2.worldPos);

            // Depth change distance
            dis += Mathf.Abs(p1.depth - p2.depth) * Position.LAYER_DISTANCE_COST;
        }

        // Add the total number of levels that will be crossed
        dis += Mathf.Abs(this.path.getPoint(this.path.pointCount - 1).depth - this.worker.depth) * Position.LAYER_DISTANCE_COST;

        return dis;
    }

    /// <summary>
    /// Sets the Worker's destination.
    /// </summary>
    /// <param name="destination">The destination in Grid units</param>
    /// <param name="stopAdjacentToFinish"> If true, the path will end on the closest adjacent cell. </param>
    /// <returns>
    /// The end point of the path (not the same as destination if stopAdjacentToFinish is true). 
    /// If the worker is at the destination, the destination is returned.
    /// If stopAdjacentToFinish is true and the destination is adjacent to the Worker, the Worker's postion is returned.
    /// If no path can be found, null is returned.
    /// </returns>
    public Position? setDestination(Position destination, bool stopAdjacentToFinish = false) {
        if(this.worker.position == destination) {
            // Worker is already at destination.
            return destination;
        }

        if(stopAdjacentToFinish && this.worker.position.distance(destination) == 1) {
            // Worker is adjacent to the finish, they are where they should be.

            // Look at the destination.
            this.worker.rotation = Rotation.directionToRotation(destination.vec2 - this.worker.worldPos);

            return destination;
        }

        // Try and find a path.
        PathPoint[] newPath = this.worker.world.navManager.findPath(
            new Position(this.worker.getCellPos(), this.worker.depth),
            destination,
            stopAdjacentToFinish);

        if(newPath == null) {
            Debug.Log(this.worker.name + " could not find a path to " + destination);
            return null;
        }

        Rotation endRotation = stopAdjacentToFinish ?
            Rotation.directionToRotation(destination.vec2Int - newPath[newPath.Length - 1].cellPos) :
            null;
        this.path = new NavPath(newPath, endRotation);

        if(newPath.Length == 0) {
            Debug.LogWarning("Path has a length of 0!");
            return destination;
        } else {
            PathPoint pp = this.path.endPoint;
            return new Position(pp.cellPos, pp.depth);
        }
    }

    public void setPathEndingRotation(Rotation rotation) {
        if(this.hasPath()) {
            this.path.endingLookDirection = rotation;
        }
    }
}