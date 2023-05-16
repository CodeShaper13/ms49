using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using NaughtyAttributes;

public class FogRenderer : MonoBehaviour {

    [SerializeField, Required]
    private World _world = null;
    [SerializeField, Required]
    private Tilemap _tilemap = null;
    [SerializeField, Required]
    private TilemapRenderer _tilemapRenderer = null;
    [SerializeField, Required]
    private TileBase _fogTile = null;
    [SerializeField]
    private KeyCode fogToggleKey = KeyCode.F5;
    [SerializeField, Range(0, 1)]
    private float creativeFogAlpha = 0.5f;

    private LinkedList<Vector3Int> dirtiedTiles;
    private Color normalTilemapColor;
    private TileBase[] cachedTileArray;

    private void Awake() {
        this.dirtiedTiles = new LinkedList<Vector3Int>();
        this.normalTilemapColor = this._tilemap.color;
        int mapSize = this._world.MapSize;
        this.cachedTileArray = new TileBase[mapSize * mapSize];

        // Bounds must be set before using TileMap#SetTilesBlock().
        int len = mapSize + 2;
        this._tilemap.size = new Vector3Int(len, len, 0);
        this._tilemap.origin = new Vector3Int(-1, -1, 0);
        this._tilemap.ResizeBounds();

        this.AddBorderFog();
    }

    private void Update() {
        if(Input.GetKeyDown(this.fogToggleKey)) {
            this._tilemapRenderer.enabled = !this._tilemapRenderer.enabled;
        }

        this._tilemap.color = CameraController.instance.inCreativeMode ?
            new Color(
                this.normalTilemapColor.r,
                this.normalTilemapColor.g,
                this.normalTilemapColor.b,
                this.creativeFogAlpha) : this.normalTilemapColor;
    }

    private void LateUpdate() {
        // Redraw only dirty Cells.
        foreach(Vector3Int v in this.dirtiedTiles) {
            this._tilemap.SetTile(
                new Vector3Int(v.x, v.y, 0),
                v.z == 1 ? this._fogTile : null);
        }

        this.dirtiedTiles.Clear();
    }

    public void Redraw(Layer layer) {

        this._tilemapRenderer.enabled = layer.HasFog;

        if(layer.HasFog) {
            int size = this._world.MapSize;
            for(int x = 0; x < size; x++) {
                for(int y = 0; y < size; y++) {
                    this.cachedTileArray[size * y + x] = layer.fog.isFogPresent(x, y) ? this._fogTile : null;
                }
            }

            this._tilemap.SetTilesBlock(
                new BoundsInt(
                    0, 0, 0,
                    size, size, 1),
                this.cachedTileArray);
        }
    }

    public void DirtyTile(int x, int y, bool visible) {
        this.dirtiedTiles.AddLast(new Vector3Int(x, y, visible ? 1 : 0));
    }

    private void AddBorderFog() {
        int mapSize = this._world.MapSize;
        int len = mapSize + 2;

        TileBase[] tiles = new TileBase[len];
        for(int i = 0; i < len; i++) {
            tiles[i] = this._fogTile;
        }

        // Left.
        this._tilemap.SetTilesBlock(
            new BoundsInt(
                -1, -1, 0,
                1, len, 1),
            tiles);

        // Right.
        this._tilemap.SetTilesBlock(
            new BoundsInt(
                mapSize, -1, 0,
                1, len, 1),
            tiles);

        // Bottom.
        this._tilemap.SetTilesBlock(
            new BoundsInt(
                -1, -1, 0,
                len, 1, 1),
            tiles);

        // Top.
        this._tilemap.SetTilesBlock(
            new BoundsInt(
                -1, mapSize, 0,
                len, 1, 1),
            tiles);
    }
}
