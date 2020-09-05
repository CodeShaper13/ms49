using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellTilemapRenderer : MonoBehaviour {

    [SerializeField]
    private Tilemap groundMap = null;
    [SerializeField]
    private Tilemap objectMap = null;
    [SerializeField]
    private Tilemap overlayMap = null;

    public int mapSize;

    public bool totalRedraw { private get; set; }
    private List<Vector2Int> dirtiedTiles;

    public Func<int, int, Color> floorTintGetterFunc;
    public Func<int, int, CellState> cellStateGetterFunc;
    public Func<int, int, TileBase> fallbackFloorGetterFunc;

    private void Awake() {
        this.dirtiedTiles = new List<Vector2Int>();
    }

    private void LateUpdate() {
        if(this.totalRedraw) {
            this.totalRedraw = false;

            // Redraw all of the Cells.
            for(int x = 0; x < this.mapSize; x++) {
                for(int y = 0; y < this.mapSize; y++) {
                    this.renderTile(x, y);
                }
            }
        }
        else {
            // Redraw only dirty Cells.
            foreach(Vector2Int v in this.dirtiedTiles) {
                this.renderTile(v.x, v.y);
            }
        }

        this.dirtiedTiles.Clear();
    }

    public void dirtyTile(int x, int y) {
        this.dirtiedTiles.Add(new Vector2Int(x, y));
    }

    public void clear() {
        if(this.groundMap != null) {
            this.groundMap.ClearAllTiles();
        }

        if(this.objectMap != null) {
            this.objectMap.ClearAllTiles();
        }

        if(this.overlayMap != null) {
            this.overlayMap.ClearAllTiles();
        }
    }

    public void renderTile(int x, int y) {
        CellState state = this.cellStateGetterFunc != null ? this.cellStateGetterFunc(x, y) : null;

        if(state == null) {
            return;
        }

        CellData data = state.data;
        Vector3Int pos = new Vector3Int(x, y, 0);
        Color groundTint = this.floorTintGetterFunc != null ? this.floorTintGetterFunc(x, y) : Color.clear;

        // Draw Ground Tile:
        if(this.groundMap != null) {
            // Draw the ground tile, or if none is specified, use the floor fallback (dirt).
            TileBase gTile; 
            if(data.groundTile != null) {
                gTile = data.groundTile;
            } else {
                gTile = this.fallbackFloorGetterFunc != null ? this.fallbackFloorGetterFunc(x, y) : null;
            }
            this.groundMap.SetTile(pos, gTile);

            // Color
            if(data.tintGroundTile || data.groundTile == null) { // Tint air
                this.colorSquare(this.groundMap, pos, groundTint);
            }
        }


        DirectionalTile dt = data.getObjectTile(state.rotation);

        // Draw Object Tile:
        if(this.objectMap != null) {
            this.objectMap.GetTileFlags(pos);

            // If the Cell's behavior implements IRenderTileOverride, let it adject the tile, even if the tile is rotatable.
            if(state.hasBehavior() && state.behavior is IRenderTileOverride) {
                ((IRenderTileOverride)state.behavior).getObjectTile(ref dt.tile);
            }

            // Set the tile on the map.
            this.objectMap.SetTile(pos, dt.tile);

            // Set the tile's transform.
            this.objectMap.SetTransformMatrix(pos, dt.getMatrix());

            // Color
            if(data.tintObjectTile) {
                this.colorSquare(this.objectMap, pos, groundTint);
            }
        }


        // Draw Overlay Tile:
        if(this.overlayMap != null) {
            this.overlayMap.SetTile(pos, dt.overlayTile);
        }
    }

    private void colorSquare(Tilemap map, Vector3Int pos, Color c) {
        map.SetTileFlags(pos, TileFlags.None);
        map.SetColor(pos, c);
    }
}
