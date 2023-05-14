using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using static BinaryTilemapRenderer;

public class FogRenderer : MonoBehaviour {

    [SerializeField]
    private Tilemap tilemap = null;
    [SerializeField]
    private TilemapRenderer tilemapRenderer = null;
    [SerializeField]
    private TileBase tile = null;
    [SerializeField]
    private KeyCode fogToggleKey = KeyCode.F5;
    [SerializeField, Range(0, 1)]
    private float creativeFogAlpha = 0.5f;

    public int mapSize { get; set; }

    private List<DirtyTile> dirtiedTiles;
    private Color normalTilemapColor;

    private void Awake() {
        this.dirtiedTiles = new List<DirtyTile>();
        this.normalTilemapColor = this.tilemap.color;
    }

    private void Update() {
        if(Input.GetKeyDown(this.fogToggleKey)) {
            this.tilemapRenderer.enabled = !this.tilemapRenderer.enabled;
        }

        this.tilemap.color = CameraController.instance.inCreativeMode ?
            new Color(
                this.normalTilemapColor.r,
                this.normalTilemapColor.g,
                this.normalTilemapColor.b,
                this.creativeFogAlpha) :
                this.normalTilemapColor;
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
    public void redraw(Layer layer) {
        this.tilemapRenderer.enabled = layer.HasFog;

        if(layer.HasFog) {
            for(int x = -1; x < mapSize + 1; x++) {
                for(int y = -1; y < mapSize + 1; y++) {
                    bool fogPresent;
                    if(x == -1 || x == mapSize | y == -1 || y == mapSize) {
                        // This is an edge, fog is always there
                        fogPresent = true;
                    } else {
                        fogPresent = layer.fog.isFogPresent(x, y);
                    }

                    Vector3Int v = new Vector3Int(x, y, 0);

                    if((this.tilemap.GetTile(v) == null) == fogPresent) {
                        this.tilemap.SetTile(v, fogPresent ? this.tile : null);
                    }
                }
            }
        }
    }

    public void clear() {
        this.tilemap.ClearAllTiles();
    }

    public void dirtyTile(int x, int y, bool visible) {
        this.dirtiedTiles.Add(new DirtyTile(new Vector2Int(x, y), visible));
    }
}
