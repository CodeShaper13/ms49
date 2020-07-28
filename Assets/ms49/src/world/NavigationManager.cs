using UnityEngine;
using System;
using System.Collections.Generic;

public class NavigationManager {

    private NavGrid[] grids;
    private Storage storage;

    private Heap<Node> cachedOpenSetHeap;

    public NavigationManager(Storage storage) {
        this.storage = storage;

        int layerCount = storage.layerCount;
        this.grids = new NavGrid[layerCount];
        for(int i = 0; i < layerCount; i++) {
            this.grids[i] = new NavGrid(this.storage.mapSize);
        }

        this.cachedOpenSetHeap = new Heap<Node>(this.storage.mapSize * this.storage.mapSize * layerCount);
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

    public PathPoint[] findPath(Position start, Position end, bool ignoreUnwalkableStartAndEnd, bool stopAdjacentToFinish) {
        Node startNode = this.getNode(start);
        Node endNode = this.getNode(end);

        //if(ignoreUnwalkableStartAndEnd || (startNode.isWalkable && endNode.isWalkable)) {
            // Both the start and end can be walked on.

            this.cachedOpenSetHeap.Clear(); // Get the heap ready to reuse

            HashSet<Node> closedSet = new HashSet<Node>();
            this.cachedOpenSetHeap.Add(startNode);

            while(this.cachedOpenSetHeap.count > 0) {
                Node currentNode = this.cachedOpenSetHeap.RemoveFirst();
                closedSet.Add(currentNode);

                if(currentNode == endNode) {
                    return this.retracePath(startNode, endNode, stopAdjacentToFinish);
                }

                foreach(Node neighbor in this.getAdjacentNodes(currentNode)) {
                    if(closedSet.Contains(neighbor)) {
                        continue; // already visited this node.
                    }
                    if(!neighbor.isWalkable) {
                        // Node can't be walked on, only continue if ignoreUnwalkableStartAndEnd is true and node is end node.
                        if(!(ignoreUnwalkableStartAndEnd == true && neighbor == endNode)) {
                            continue;
                        }
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + this.getDistance(currentNode, neighbor);
                    if(newMovementCostToNeighbour < neighbor.gCost || !this.cachedOpenSetHeap.Contains(neighbor)) {
                        neighbor.gCost = newMovementCostToNeighbour;
                        neighbor.hCost = this.getDistance(neighbor, endNode);
                        neighbor.parent = currentNode;

                        if(!this.cachedOpenSetHeap.Contains(neighbor)) {
                            this.cachedOpenSetHeap.Add(neighbor);
                        }
                    }
                }
            //}
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

        PathPoint[] waypoints;
        if(path.Count == 1) {
            waypoints = new PathPoint[] { path[0].asPathPoint() };
        }
        else {
            waypoints = new PathPoint[path.Count];
            for(int i = 0; i < path.Count; i++) {
                waypoints[i] = path[i].asPathPoint();
            }
            Array.Reverse(waypoints);
        }

        return waypoints;
    }

    private int getDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if(dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
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
