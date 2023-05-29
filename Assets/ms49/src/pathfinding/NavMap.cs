using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NavMap : MonoBehaviour {

    [SerializeField, Required]
    private World _world = null;
    [SerializeField, Min(0)]
    private int _navId = 0;

    private NavGrid[] grids;

    public int NavId => this._navId;

    private void OnValidate() {
        this._navId = Mathf.Clamp(this._navId, 0, Pathfinder.MAX_NAV_COUNT);
    }

    private void Start() {
        int mapSize = this._world.MapSize;
        int layerCount = this._world.storage.layerCount;

        this.grids = new NavGrid[layerCount];
        for(int i = 0; i < layerCount; i++) {
            this.grids[i] = new NavGrid(mapSize);
        }

        Pathfinder.RegisterNavMap(this);
    }

    private void Update() {
        for(int i = 0; i < this._world.storage.layerCount; i++) {
            Layer layer = this._world.storage.GetLayer(i);
            if(layer.navGridDirty) {
                this.grids[i].Bake(layer);
            }
        }
    }

    private void OnDestroy() {
        Pathfinder.UnregisterNavMap(this);
    }

    private void OnDrawGizmosSelected() {
        if(CameraController.instance != null) { // Game is in progress
            NavGrid grid = this.grids[CameraController.instance.currentLayer];

            foreach(PathfindingNode node in grid.nodes) {
                Gizmos.color = node.IsWalkable ? Color.green : Color.red;
                Gizmos.DrawSphere(node.Position.Center, 0.25f);
            }
        }
    }

    public PathfindingNode GetNode(Position pos) { // TODO add safety checks
        return this.grids[pos.depth].GetNode(pos.AsVec2Int);
    }

    /// <summary>
    /// Returns a List of all connecting nodes.
    /// </summary>
    public void GetAdjacentNodes(PathfindingNode node, PathfindingNode[] cachedNeighborArray) {
        int counter = 0;

        for(int i = 0; i < 4; i++) {
            Rotation r = Rotation.ALL[i];
            int checkX = node.x + r.xDir;
            int checkY = node.y + r.yDir;

            if(this.grids[node.depth].InMap(checkX, checkY)) {
                cachedNeighborArray[counter] = this.grids[node.depth].nodes[checkX, checkY];
                counter++;
            }
        }

        if(node.ConnectsUp) {
            cachedNeighborArray[counter] = this.grids[node.depth - 1].nodes[node.x, node.y];
            counter++;
        }

        if(node.ConnectsDown) {
            cachedNeighborArray[counter] = this.grids[node.depth + 1].nodes[node.x, node.y];
            counter++;
        }

        for(int i = counter; i < cachedNeighborArray.Length; i++) {
            cachedNeighborArray[i] = null;
        }
    }

    private class NavGrid {

        public PathfindingNode[,] nodes;

        private int mapSize;

        public NavGrid(int size) {
            this.nodes = new PathfindingNode[size, size];
            this.mapSize = size;
        }

        /// <summary>
        /// Returns the node at the passed position.
        /// </summary>
        public PathfindingNode GetNode(Vector2Int pos) {
            return this.nodes[pos.x, pos.y];
        }

        /// <summary>
        /// Returns a List of all adjacent nodes.
        /// </summary>
        public List<PathfindingNode> GetAdjacentNodes(PathfindingNode node) {
            List<PathfindingNode> neighbours = new List<PathfindingNode>();

            for(int x = -1; x <= 1; x++) {
                for(int y = -1; y <= 1; y++) {
                    if(x == 0 && y == 0) {
                        continue;
                    }

                    // Prevent diagonals from being added.
                    if(Math.Abs(x) == Math.Abs(y)) {
                        continue;
                    }

                    int checkX = node.x + x;
                    int checkY = node.y + y;

                    if(this.InMap(checkX, checkY)) {
                        neighbours.Add(this.nodes[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public void Bake(Layer layer) {
            for(int x = 0; x < this.mapSize; x++) {
                for(int y = 0; y < this.mapSize; y++) {
                    CellData data = layer.GetCellState(x, y).data;
                    this.nodes[x, y] = new PathfindingNode(
                        x,
                        y,
                        layer.depth,
                        data.ZMoveDirections,
                        data.MovementCost);
                }
            }

            layer.navGridDirty = false;
        }

        public bool InMap(int x, int y) {
            return x >= 0 && x < this.mapSize && y >= 0 && y < this.mapSize;
        }
    }
}