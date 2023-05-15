using UnityEngine;

[RequireComponent(typeof(EntityBase))]
[DisallowMultipleComponent]
public class PathfindingAgent : MonoBehaviour {

    [Min(0), Tooltip("How many units the Entity moves per second.")]
    public float moveSpeed = 1f;
    [Min(0), Tooltip("How long it takes to climb a ladder in seconds.")]
    public float ladderClimbSpeed = 0.1f;
    [Min(0), Tooltip("What NavMap this Agent Operates on.")]
    public int navMapId = 0;

    private EntityBase entity;
    private float layerChangeProgress;

    public NavPath path { get; private set; }

    private void Start() {
        this.entity = this.GetComponent<EntityBase>();
    }

    private void Update() {
        if(this.HasPath()) {
            Position currentWaypoint = this.path.TargetPoint;
            Vector2 workerPos = this.entity.worldPos;

            bool posMatch = workerPos == currentWaypoint.Center;
            bool depthMatch = entity.depth == currentWaypoint.depth;

            if(posMatch) {
                if(depthMatch) {
                    this.path.NextPoint();
                    if(this.path.targetIndex >= path.PointCount) {
                        // Reached the end of the path.

                        if(this.path.endingLookDirection != null) {
                            this.entity.rotation = this.path.endingLookDirection;
                        }

                        this.Stop();

                        return;
                    }
                    currentWaypoint = this.path.TargetPoint;
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

                    if(this.layerChangeProgress > this.ladderClimbSpeed) {
                        this.layerChangeProgress = 0;
                        this.entity.depth = currentWaypoint.depth;
                    }
                }
            }

            Vector3 posBeforeMove = this.transform.position;

            // Move the Worker towards the next point on the X and Y axis.
            this.transform.position = Vector2.MoveTowards(
                transform.position,
                currentWaypoint.Center,
                this.moveSpeed * Time.deltaTime);

            // Update the direction the Worker is looking.
            Vector2 direction = this.transform.position - posBeforeMove;
            this.entity.rotation = Rotation.directionToRotation(direction);
        }
    }

    private void OnDrawGizmos() {
        if(this.HasPath()) {
            for(int i = this.path.targetIndex; i < this.path.PointCount; i++) {
                Gizmos.color = Color.yellow;

                if(i == this.path.targetIndex) {
                    Gizmos.DrawLine(
                        this.transform.position,
                        this.path[i].Center);
                } else {
                    Gizmos.DrawLine(
                        this.path[i - 1].Center,
                        this.path[i].Center);
                }
            }

            for(int i = 0; i < this.path.PointCount; i++) {
                Gizmos.DrawSphere(this.path[i].Center, 0.1f);
            }
        }
    }

    /// <summary>
    /// Stops the Agent.  This will delete the path.
    /// </summary>
    public void Stop() {
        this.path = null;
    }

    /// <summary>
    /// Returns true if the Agent has a path that they are following.
    /// </summary>
    public bool HasPath() {
        return this.path != null;
    }

    public void SetPath(NavPath path) {
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
    public NavPath CalculatePath(Position destination, bool stopAdjacentToFinish = false, Rotation endingRotation = null) {
        if(this.entity.position == destination) {
            if(stopAdjacentToFinish) {
                //dest = free position adjacent to entity
                World w = this.entity.world;
                foreach(Rotation r in Rotation.ALL) {
                    Position p = destination + r;
                    if(!w.IsOutOfBounds(p) && w.GetCellState(p).data.IsWalkable) {
                        destination = p;
                        stopAdjacentToFinish = false;
                        // TODO looking rot?
                    }
                }
            } else {
                return new NavPath(
                    new Position[] { destination },
                    endingRotation);
            }
        } else if(Vector2Int.Distance(this.entity.position.AsVec2Int, destination.AsVec2Int) == 1) { // Next to destination
            if(stopAdjacentToFinish) {
                return new NavPath(
                    new Position[] { this.entity.position }, // Go to their own spot
                    Rotation.directionToRotation(destination.AsVec2Int - this.entity.getCellPos()));
            }
        }

        // Return if either the start of the end is out of the world
        if(this.entity.world.IsOutOfBounds(this.entity.position) || this.entity.world.IsOutOfBounds(destination)) {
            return null;
        }

        // Try and find a path.
        Position[] pathPoints = Pathfinder.FindPath(
            this.navMapId,
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
                Rotation.directionToRotation(destination.AsVec2Int - pathPoints[pathPoints.Length - 1].AsVec2Int) :
                null;
        }

        return new NavPath(pathPoints, endRot);
    }
}