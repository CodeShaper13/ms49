using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class BinaryTilemapRenderer : MonoBehaviour {

    [SerializeField]
    private Tilemap tilemap = null;
    [SerializeField]
    private TileBase tile = null;
    [SerializeField]
    public int mapSize;

    private List<DirtyTile> dirtiedTiles;

    private void Awake() {
        this.dirtiedTiles = new List<DirtyTile>();
    }

    private void LateUpdate() {
        // Redraw only dirty Cells.
        foreach(DirtyTile t in this.dirtiedTiles) {
            this.tilemap.SetTile(
                new Vector3Int(t.position.x, t.position.y, 0),
                t.visible ? this.tile : null);
        }

        this.dirtiedTiles.Clear();
    }

    /// <summary>
    /// Sets the Renderer's tile getter function and clears the map for a total redraw.
    /// </summary>
    public void redraw(Func<int, int, bool> function) {
        this.clear();

        for(int x = 0; x < mapSize; x++) {
            for(int y = 0; y < mapSize; y++) {

                bool flag = function(x, y);
                this.tilemap.SetTile(new Vector3Int(x, y, 0), flag ? this.tile : null);
            }
        }
    }

    public void clear() {
        this.tilemap.ClearAllTiles();
    }

    public void setTile(int x, int y, bool visible) {
        this.dirtiedTiles.Add(new DirtyTile(new Vector2Int(x, y), visible));
    }

    public struct DirtyTile {

        public Vector2Int position;
        public bool visible;

        public DirtyTile(Vector2Int position, bool visible) {
            this.position = position;
            this.visible = visible;
        }
    }
}
