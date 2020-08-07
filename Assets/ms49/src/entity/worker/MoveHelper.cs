using UnityEngine;

[RequireComponent(typeof(EntityWorker))]
public class MoveHelper : MonoBehaviour {

    [SerializeField]
    [Tooltip("The Worker's speed in units per second.")]
    private float speed = 1f;

    private EntityWorker worker;

    private PathPoint[] path;
    private int targetIndex;

    private void Awake() {
        this.worker = this.GetComponent<EntityWorker>();
    }

    public void update() {
        if(this.hasPath()) {
            PathPoint currentWaypoint = this.path[this.targetIndex];
            Vector3 workerPos = this.worker.worldPos;
            if(workerPos.x == currentWaypoint.x && workerPos.y == currentWaypoint.y) {
                this.targetIndex++;
                if(this.targetIndex >= path.Length) {
                    this.stop();
                    return;
                }
                currentWaypoint = this.path[this.targetIndex];
            }

            // Move the Worker towards the next point on the X and Y axis.
            this.transform.position = Vector2.MoveTowards(
                transform.position,
                currentWaypoint.worldPos,
                speed * Time.deltaTime);

            // Change the Workers Z
            if(currentWaypoint.depth != this.worker.depth) {
                this.worker.depth = currentWaypoint.depth;
            }
        }
    }

    private void OnDrawGizmos() {
        if(this.hasPath()) {
            for(int i = targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.yellow;

                if(i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i].worldPos);
                } else {
                    Gizmos.DrawLine(path[i - 1].worldPos, path[i].worldPos);
                }
            }
            
            foreach(PathPoint point in this.path) {
                Gizmos.DrawSphere(point.worldPos, 0.1f);
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
        return this.path != null && this.path.Length > 0;
    }

    public NavPath getPath() {
        NavPath p = new NavPath(this.path);
        p.targetIndex = this.targetIndex;
        return p;
    }

    /// <summary>
    /// Returns the distance from the Worker to the end of the path in a straight line.
    /// -1 is returned if the Worker has no path.
    /// </summary>
    public float getDirectDistanceToEnd() {
        if(!this.hasPath()) {
            return -1f;
        }

        float dis = Vector2.Distance(this.worker.getCellPos(), this.path[this.path.Length - 1].worldPos);

        // Add the total number of levels that will be crossed
        dis += Mathf.Abs(this.path[this.path.Length - 1].depth - this.worker.depth) * Position.LAYER_DISTANCE_COST;

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
        float dis = Vector2.Distance(this.worker.getCellPos(), this.path[this.targetIndex].worldPos);
        dis += Mathf.Abs(this.worker.depth - this.path[this.targetIndex].depth) * Position.LAYER_DISTANCE_COST;

        // Total the distances between the nodes.
        for(int i = this.targetIndex; i < this.path.Length - 1; i++) {
            PathPoint p1 = this.path[i];
            PathPoint p2 = this.path[i + 1];

            // X and Y distance.
            dis += Vector2.Distance(p1.worldPos, p2.worldPos);

            // Depth change distance
            dis += Mathf.Abs(p1.depth - p2.depth) * Position.LAYER_DISTANCE_COST;
        }

        // Add the total number of levels that will be crossed
        dis += Mathf.Abs(this.path[this.path.Length - 1].depth - this.worker.depth) * Position.LAYER_DISTANCE_COST;

        return dis;
    }

    /// <summary>
    /// Sets the Worker's destination.
    /// </summary>
    /// <param name="destination">The destination in Grid units</param>
    /// <param name="stopAdjacentToFinish"> If true, the path will end on the closest adjacent cell. </param>
    /// <returns> The end point of the path (not the same as destination if stopAdjacentToFinish is true).  If no path can be found, null is returned. </returns>
    public Position? setDestination(Position destination, bool stopAdjacentToFinish = false) {
        PathPoint[] newPath = this.worker.world.navManager.findPath(
            new Position(this.worker.getCellPos(), this.worker.depth),
            destination,
            stopAdjacentToFinish);

        if(newPath == null) {
            Debug.Log("No path found");
            return null;
        }
        else {
            this.path = newPath;
            this.targetIndex = 0;

            if(newPath.Length == 0) {
                return destination;
            } else {
                PathPoint pp = newPath.Length == 1 ? newPath[0] : newPath[newPath.Length - 1];
                return new Position((int)pp.x, (int)pp.y, pp.depth);
            }
        }
    }
}