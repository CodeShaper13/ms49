﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellTilemapRenderer : MonoBehaviour {

    [SerializeField]
    private Tilemap floorTilemap = null;
    [SerializeField]
    private Tilemap floorOverlayTilemap = null;
    [SerializeField]
    private Tilemap objectMap = null;
    [SerializeField]
    private Tilemap overlayMap = null;

    public bool totalRedraw { private get; set; }
    private List<Vector2Int> dirtiedCells;

    private int mapSize;
    private Func<int, int, Color> floorTintGetterFunc;
    private Func<int, int, Color> hardnessColorModGetterFunc;
    private Func<int, int, CellState> cellStateGetterFunc;
    private Func<int, int, TileBase> floorGetterFunc;

    private void Awake() {
        this.dirtiedCells = new List<Vector2Int>();
    }

    public void initializedRenderer(int mapSize, Func<int, int, Color> floorTintGetterFunc, Func<int, int, Color> hardnessColorModGetterFunc, Func<int, int, CellState> cellStateGetterFunc, Func<int, int, TileBase> floorGetterFunc) {
        this.mapSize = mapSize;
        this.floorTintGetterFunc = floorTintGetterFunc;
        this.hardnessColorModGetterFunc = hardnessColorModGetterFunc;
        this.cellStateGetterFunc = cellStateGetterFunc;
        this.floorGetterFunc = floorGetterFunc;
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
            foreach(Vector2Int v in this.dirtiedCells) {
                this.renderTile(v.x, v.y);
            }
        }

        this.dirtiedCells.Clear();
    }

    /// <summary>
    /// Dirties the Cell at the passed position so it is redrawn at
    /// the end of the frame.
    /// </summary>
    public void dirtyCell(int x, int y) {
        this.dirtiedCells.Add(new Vector2Int(x, y));
    }

    /// <summary>
    /// Clears all of the Tilemaps.
    /// </summary>
    public void clear() {
        this.clearIfNotNull(this.floorTilemap);
        this.clearIfNotNull(this.floorOverlayTilemap);
        this.clearIfNotNull(this.objectMap);
        this.clearIfNotNull(this.overlayMap);
    }

    private void renderTile(int x, int y) {
        CellState state = this.cellStateGetterFunc != null ? this.cellStateGetterFunc(x, y) : null;

        if(state == null) {
            return;
        }

        CellData data = state.data;
        Vector3Int pos = new Vector3Int(x, y, 0);

        // Draw the floor:
        if(this.floorTilemap != null) {
            Color floorTint = this.floorTintGetterFunc != null ? this.floorTintGetterFunc(x, y) : Color.white;

            TileBase tile;
            if(data.isSolid) {
                // Floor not visible
                tile = null;
            } else {
                // Floor is visible, draw dirt/grass
                tile = this.floorGetterFunc != null ? this.floorGetterFunc(x, y) : null;
            }

            this.floorTilemap.SetTile(pos, tile);
            this.floorTilemap.SetColor(pos, floorTint);
        }

        TileRenderData dt = data.getRenderData(state.rotation);


        // If the Cell's behavior implements IRenderTileOverride, let it adject the tile, even if the tile is rotatable.
        if(state.hasBehavior() && state.behavior is IRenderTileOverride) {
            ((IRenderTileOverride)state.behavior).replaceTiles(ref dt);
        }


        // Draw floor overlay:
        if(this.floorOverlayTilemap != null) {
            this.floorOverlayTilemap.SetTile(pos, dt.floorOverlayTile);
        }


        // Draw Object Tile:
        if(this.objectMap != null) {
            this.objectMap.GetTileFlags(pos);

            // Set the tile on the map.
            this.objectMap.SetTile(pos, dt.objectTile);

            // Set the tile's transform.
            this.objectMap.SetTransformMatrix(pos, dt.getMatrix());

            // Color
            if(data.recieveHardnessColorMod) {
                Color hardnessColorMod = this.hardnessColorModGetterFunc != null ? this.hardnessColorModGetterFunc(x, y) : Color.white;
                this.colorSquare(this.objectMap, pos, hardnessColorMod);
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

    private void clearIfNotNull(Tilemap tilemap) {
        if(tilemap != null) {
            tilemap.ClearAllTiles();
        }
    }
}
