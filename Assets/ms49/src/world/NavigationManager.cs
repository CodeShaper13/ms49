using UnityEngine;
using System;
using System.Collections.Generic;

public class NavigationManager {

    private NavGrid[] grids;
    private World world;
    private int mapSize;

    private Heap<Node> openSet;

    public NavigationManager(World world, int mapSize) {
        this.world = world;
        this.mapSize = mapSize;

        int layerCount = this.world.storage.layerCount;
        this.grids = new NavGrid[layerCount];
        for(int i = 0; i < layerCount; i++) {
            this.grids[i] = new NavGrid(mapSize);
        }

        this.openSet = new Heap<Node>(mapSize * mapSize * layerCount);
    }

    public void update() {
        // Update Layers
        for(int i = 0; i < this.world.storage.layerCount; i++) {
            this.rebakeNavGridIfDirty(this.grids[i], this.world.storage.getLayer(i));
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

        //this.openSet = new Heap<Node>(this.mapSize * this.mapSize * this.world.storage.layerCount); //.Clear(); // Get the heap ready to reuse
        this.openSet.Clear();
        HashSet<Node> closedSet = new HashSet<Node>();
        this.openSet.Add(startNode);

        Node[] cachedNeighborArray = new Node[7];

        while(this.openSet.count > 0) {
            Node currentNode = this.openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode == endNode) {
                return this.retracePath(startNode, endNode, stopAdjacentToFinish);
            }

            this.getAdjacentNodes(currentNode, cachedNeighborArray);
            foreach(Node neighbor in cachedNeighborArray) {
                if(neighbor == null) {
                    break; // Reached the "end" of the array
                }

                if(closedSet.Contains(neighbor)) {
                    continue; // already visited this node.
                }

                if(!neighbor.isWalkable && neighbor != endNode) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + this.getDistance(currentNode, neighbor) + neighbor.movementPenalty;
                if(newMovementCostToNeighbour < neighbor.gCost || !this.openSet.Contains(neighbor)) {
                    neighbor.gCost = newMovementCostToNeighbour;
                    neighbor.hCost = this.getDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if(!this.openSet.Contains(neighbor)) {
                        this.openSet.Add(neighbor);
                    } else {
                        this.openSet.UpdateItem(neighbor);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a List of all connecting nodes.
    /// </summary>
    private void getAdjacentNodes(Node node, Node[] cachedNeighborArray) {
        int counter = 0;

        for(int i = 0; i < 4; i++) {
            Rotation r = Rotation.ALL[i];
            int checkX = node.x + r.xDir;
            int checkY = node.y + r.yDir;

            if(this.grids[node.depth].inMap(checkX, checkY)) {
                cachedNeighborArray[counter] = this.grids[node.depth].nodes[checkX, checkY];
                counter++;
            }
        }

        if(node.connectsUp()) {
            cachedNeighborArray[counter] = this.grids[node.depth - 1].nodes[node.x, node.y];
            counter++;
        }

        if(node.connectsDown()) {
            cachedNeighborArray[counter] = this.grids[node.depth + 1].nodes[node.x, node.y];
            counter++;
        }

        for(int i = counter; i < cachedNeighborArray.Length; i++) {
            cachedNeighborArray[i] = null;
        }
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

        int distX = Math.Abs(nodeA.x - nodeB.x);
        int distZ = Math.Abs(nodeA.depth - nodeB.depth);
        int distY = Math.Abs(nodeA.y - nodeB.y);

        if(distX > distZ) {
            return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
        } else {
            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }
    }

    private void rebakeNavGridIfDirty(NavGrid grid, Layer layer) {
        if(layer.navGridDirty) {

            // Regenerate the NavGrid.
            int size = layer.size;
            Node[,] nodes = grid.nodes;
            for(int x = 0; x < size; x++) {
                for(int y = 0; y < size; y++) {
                    Vector2 worldPoint = new Vector2(x + 0.5f, y + 0.5f);
                    CellData data = layer.getCellState(x, y).data;
                    nodes[x, y] = new Node(
                        worldPoint,
                        x,
                        y,
                        layer.depth,
                        data.zMoveDirections,
                        data.movementCost);
                }
            }
            grid.nodes = nodes;

            layer.navGridDirty = false;
        }
    }
}
