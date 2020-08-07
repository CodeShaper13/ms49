using UnityEngine;
using System;
using System.Collections.Generic;

public class NavigationManager {

    private NavGrid[] grids;
    private Storage storage;

    private Heap<Node> openSet;

    public NavigationManager(Storage storage) {
        this.storage = storage;

        int layerCount = storage.layerCount;
        this.grids = new NavGrid[layerCount];
        for(int i = 0; i < layerCount; i++) {
            this.grids[i] = new NavGrid(this.storage.mapSize);
        }

        this.openSet = new Heap<Node>(this.storage.mapSize * this.storage.mapSize * layerCount);
    }

    public void update() {
        // Update Layers
        for(int i = 0; i < this.storage.layerCount; i++) {
            this.rebakeNavGridIfDirty(this.grids[i], this.storage.getLayer(i));
        }
    }

    public void debugDraw() {
        this.grids[CameraController.instance.currentLayer].debugDraw();
    }

    public Node getNode(Position pos) { // TODO add safety checks
        return this.grids[pos.depth].getNode(pos.vec2);
    }

    public PathPoint[] findPath(Position start, Position end, bool stopAdjacentToFinish) {
        Node startNode = this.getNode(start);
        Node endNode = this.getNode(end);

        this.openSet = new Heap<Node>(this.storage.mapSize * this.storage.mapSize * this.storage.layerCount); //.Clear(); // Get the heap ready to reuse
        HashSet<Node> closedSet = new HashSet<Node>();
        this.openSet.Add(startNode);

        while(this.openSet.count > 0) {
            Node currentNode = this.openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode == endNode) {
                return this.retracePath(startNode, endNode, stopAdjacentToFinish);
            }

            foreach(Node neighbor in this.getAdjacentNodes(currentNode)) {
                if(closedSet.Contains(neighbor)) {
                    continue; // already visited this node.
                }

                if(!neighbor.isWalkable && neighbor != endNode) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + this.getDistance(currentNode, neighbor);
                if(newMovementCostToNeighbour < neighbor.gCost || !this.openSet.Contains(neighbor)) {
                    neighbor.gCost = newMovementCostToNeighbour;
                    neighbor.hCost = this.getDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if(!this.openSet.Contains(neighbor)) {
                        this.openSet.Add(neighbor);

                        //Debug.DrawLine(currentNode.worldPosition, neighbor.worldPosition, Color.white, 3);
                        //Vector3 v = neighbor.worldPosition;
                        //Debug.DrawLine(v + (Vector3.up + Vector3.right) * 0.1f, v + (Vector3.down + Vector3.left) * 0.1f, Color.black, 3f);
                        //Debug.DrawLine(v + (Vector3.down + Vector3.right) * 0.1f, v + (Vector3.up + Vector3.left) * 0.1f, Color.black, 3f);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a List of all connecting nodes.
    /// </summary>
    private List<Node> getAdjacentNodes(Node node) {
        List<Node> neighbours = new List<Node>();

        foreach(Rotation r in Rotation.ALL) {
            int checkX = node.x + r.vector.x;
            int checkY = node.y + r.vector.y;

            if(this.grids[node.depth].inMap(checkX, checkY)) {
                neighbours.Add(this.grids[node.depth].nodes[checkX, checkY]);
            }
        }

        if(node.connectsUp()) {
            neighbours.Add(this.grids[node.depth - 1].nodes[node.x, node.y]);
        }

        if(node.connectsDown()) {
            neighbours.Add(this.grids[node.depth + 1].nodes[node.x, node.y]);
        }

        return neighbours;
    }

    private PathPoint[] retracePath(Node startNode, Node endNode, bool stopAdjacentToFinish) {
        if(startNode == endNode) {
            return null;
        }        

        List<Node> path = new List<Node>();
        Node currentNode = stopAdjacentToFinish ? endNode.parent : endNode;

        while(currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        if(path.Count == 0) {
            // The path has a length of 1 (destination is an adjacent cell) and stopAdjacentToFinish is true.
            return new PathPoint[] { startNode.asPathPoint() };
        }

        // Convert the List of Nodes into an Array of PathPoints.
        PathPoint[] waypoints = new PathPoint[path.Count];
        for(int i = 0; i < path.Count; i++) {
            waypoints[i] = path[i].asPathPoint();
        }
        Array.Reverse(waypoints);


        return waypoints;
    }

    private int getDistance(Node nodeA, Node nodeB) {
        /*
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if(dstX > dstY) {
            return 14 * dstY + 10 * (dstX - dstY);
        } else {
            return 14 * dstX + 10 * (dstY - dstX);
        }
        */

        int distX = Mathf.Abs(nodeA.x - nodeB.x);
        int distZ = Mathf.Abs(nodeA.depth - nodeB.depth);
        int distY = Mathf.Abs(nodeA.y - nodeB.y);

        if(distX > distZ) {
            return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
        } else {
            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }
    }

    private void rebakeNavGridIfDirty(NavGrid grid, Layer layer) {
        if(layer.navGridDirty) {

            // Regenerate the NavGrid.
            int size = this.storage.mapSize;
            Node[,] nodes = grid.nodes;
            for(int x = 0; x < size; x++) {
                for(int y = 0; y < size; y++) {
                    Vector2 worldPoint = new Vector2(x + 0.5f, y + 0.5f);
                    CellData data = layer.getCellState(x, y).data;
                    nodes[x, y] = new Node(
                        data.isWalkable,
                        worldPoint,
                        x,
                        y,
                        layer.depth,
                        data.zMoveDirections);
                }
            }
            grid.nodes = nodes;

            layer.navGridDirty = false;
        }
    }
}
