using UnityEngine;

[RequireComponent(typeof(EntityBase))]
[DisallowMultipleComponent]
public class PathfindingAgent : MonoBehaviour {

    [SerializeField, Min(0)]
    [Tooltip("How many units the Entity moves per second.")]
    private float speed = 1f;
    [SerializeField, Min(0)]
    [Tooltip("How long it takes to climb a ladder in seconds.")]
    private float _ladderClimbSpeed = 0.1f;
    
    private EntityBase entity;
    private float layerChangeProgress;

    public NavPath path { get; private set; }

    private void Awake() {
        this.entity = this.GetComponent<EntityBase>();
    }

    public void update() {
        if(this.hasPath()) {
            PathPoint currentWaypoint = this.path.targetPoint;
            Vector2 workerPos = this.entity.worldPos;

            bool posMatch = workerPos == currentWaypoint.worldPos;
            bool depthMatch = entity.depth == currentWaypoint.depth;

            if(posMatch) {
                if(depthMatch) {
                    this.path.nextPoint();
                    if(this.path.targetIndex >= path.pointCount) {
                        // Reached the end of the path.

                        if(this.path.endingLookDirection != null) {
                            this.entity.rotation = this.path.endingLookDirection;
                        }

                        this.stop();

                        return;
                    }
                    currentWaypoint = this.path.targetPoint;
                } else {
                    if(this.layerChangeProgress == 0) {
                        // First frame climbing, start an animation
                        if(this.entity.depth > currentWaypoint.depth) {
                            //this.worker.animator.playClip("ClimbLadderUp");
                        } else {
                            //this.worker.animator.playClip("ClimbLadderDown");
                        }
                    }

                    this.layerChangeProgress += Time.deltaTime;

                    if(this.layerChangeProgress > this._ladderClimbSpeed) {
                        this.layerChangeProgress = 0;
                        this.entity.depth = currentWaypoint.depth;
                    }
                }
            }

            Vector3 posBeforeMove = this.transform.position;

            // Move the Worker towards the next point on the X and Y axis.
            this.transform.position = Vector2.MoveTowards(
                transform.position,
                currentWaypoint.worldPos,
                this.speed * Time.deltaTime);

            // Update the direction the Worker is looking.
            Vector2 direction = this.transform.position - posBeforeMove;
            this.entity.rotation = Rotation.directionToRotation(direction);
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

    public void setPath(NavPath path) {
        this.path = path;
    }

    /*
    /// <summary>
    /// Returns the distance from the Worker to the end of the path in a straight line.
    /// -1 is returned if the Worker has no path.
    /// </summary>
    public float getDirectDistanceToEnd() {
        if(!this.hasPath()) {
            return -1f;
        }

        float dis = Vector2.Distance(this.entity.getCellPos(), this.path.endPoint.worldPos);

        // Add the total number of levels that will be crossed
        dis += Mathf.Abs(this.path.endPoint.depth - this.entity.depth) * Position.LAYER_DISTANCE_COST;

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
        float dis = Vector2.Distance(this.entity.getCellPos(), this.path.targetPoint.worldPos);
        dis += Mathf.Abs(this.entity.depth - this.path.targetPoint.depth) * Position.LAYER_DISTANCE_COST;

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
        dis += Mathf.Abs(this.path.getPoint(this.path.pointCount - 1).depth - this.entity.depth) * Position.LAYER_DISTANCE_COST;

        return dis;
    }
    */

    /// <summary>
    /// Attempts to calculate a path to the passed destination.
    /// </summary>
    public NavPath calculatePath(Position destination, bool stopAdjacentToFinish = false, Rotation endingRotation = null) {
        if(this.entity.position == destination) {
            if(stopAdjacentToFinish) {
                //dest = free position adjacent to entity
                World w = this.entity.world;
                foreach(Rotation r in Rotation.ALL) {
                    Position p = destination + r;
                    if(!w.isOutOfBounds(p) && w.getCellState(p).data.isWalkable) {
                        destination = p;
                        stopAdjacentToFinish = false;
                        // TODO looking rot?
                    }
                }
            } else {
                return new NavPath(
                    new PathPoint[] { new PathPoint(destination) },
                    endingRotation);
            }
        } else if(Vector2Int.Distance(this.entity.position.vec2Int, destination.vec2Int) == 1) { // Next to destination
            if(stopAdjacentToFinish) {
                return new NavPath(
                    new PathPoint[] { new PathPoint(this.entity.position) }, // Go to their own spot
                    Rotation.directionToRotation(destination.vec2Int - this.entity.getCellPos()));
            }
        }

        // Try and find a path.
        PathPoint[] pathPoints = this.entity.world.navManager.findPath(
            this.entity.position,
            destination,
            stopAdjacentToFinish);

        if(pathPoints == null) {
            // Unable to find a path, return null.
            //Debug.Log(this.entity.name + " could not find a path to " + destination);
            return null;
        }

        Rotation endRot;
        if(endingRotation != null) {
            endRot = endingRotation;
        } else {
            endRot = stopAdjacentToFinish ?
                Rotation.directionToRotation(destination.vec2Int - pathPoints[pathPoints.Length - 1].cellPos) :
                null;
        }

        return new NavPath(pathPoints, endRot);
    }
}