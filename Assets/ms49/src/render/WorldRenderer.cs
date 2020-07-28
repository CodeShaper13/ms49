using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldRenderer : MonoBehaviour {

    public static WorldRenderer instance;

    [SerializeField]
    private World world = null;
    [SerializeField]
    private Tilemap groundMap = null;
    [SerializeField]
    private Tilemap objectMap = null;
    [SerializeField]
    private Tilemap overlayMap = null;
    [SerializeField]
    private Tilemap excavationTargetTilemap = null;
    [SerializeField]
    private Tilemap fogTilemap = null;

    [Space]

    [SerializeField]
    private TileBase targetedCellTile = null;
    [SerializeField]
    private TileBase fogTile = null;

    /// <summary> The Layer the renderer is rendering. </summary>
    private Layer layer;
    private List<Vector2Int> dirtiedTiles;
    private bool totalRedraw;

    private void Awake() {
        WorldRenderer.instance = this;

        this.dirtiedTiles = new List<Vector2Int>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F4)) {
            this.totalRedraw = true;
            Debug.Log("Redrawing Map");
        }
    }

    private void LateUpdate() {
        // Only show Entities that at the depth being rendered.
        foreach(EntityBase e in this.world.entityList) {
            if(e.depth == this.layer.depth) {
                e.gameObject.SetActive(true);
            } else {
                e.gameObject.SetActive(false);
            }
        }

        // Redraw Cells if needed.
        if(this.totalRedraw) {
            // Redraw all of the Cells.
            this.totalRedraw = false;
            int mapSize = this.world.storage.mapSize;

            for(int x = 0; x < mapSize; x++) {
                for(int y = 0; y < mapSize; y++) {
                    this.renderTile(x, y);
                }
            }

            // Redraw all of the targeted for excavation tiles
            foreach(Position p in this.world.storage.targetedForRemovalSquares) {
                if(p.depth == this.layer.depth) {
                    this.setExcavationTargetVisability(p.x, p.y, true);
                }
            }

            // Redraw all fog
            if(this.layer.hasFog()) {
                for(int x = 0; x < mapSize; x++) {
                    for(int y = 0; y < mapSize; y++) {
                        this.redrawFog(x, y, this.layer.fog.isFogPresent(x, y));
                    }
                }
            }

        } else {
            // Redraw only dirty Cells.
            foreach(Vector2Int v in this.dirtiedTiles) {
                this.renderTile(v.x, v.y);
            }
        }

        this.dirtiedTiles.Clear();
    }

    public void dirtyTile(int x, int y) {
        this.dirtyTile(new Vector2Int(x, y));
    }

    public void dirtyTile(Vector2Int position) {
        this.dirtiedTiles.Add(position);
    }

    public void redrawFog(int x, int y, bool visible) {
        this.fogTilemap.SetTile(new Vector3Int(x, y, 0), visible ? this.fogTile : null);
    }

    public void setExcavationTargetVisability(int x, int y, bool highlighted) {
        Vector3Int v = new Vector3Int(x, y, 0);
        this.excavationTargetTilemap.SetTile(v, highlighted ? this.targetedCellTile : null);
    }

    public void setLayer(Layer layer) {
        this.layer = layer;

        // Redraw the entire tilemap later on.
        this.totalRedraw = true;

        // Clear everything
        this.groundMap.ClearAllTiles();
        this.objectMap.ClearAllTiles();
        this.overlayMap.ClearAllTiles();
        this.excavationTargetTilemap.ClearAllTiles();
        this.fogTilemap.ClearAllTiles();
    }

    private void renderTile(int x, int y) {
        CellState state = layer.getCellState(x, y);
        CellData data = state.data;
        Vector3Int pos = new Vector3Int(x, y, 0);
        LayerDataBase layerData = this.world.mapGenData.getLayerFromDepth(this.layer.depth);

        // Ground Tile
        if(data.groundTile != null) {
            this.groundMap.SetTile(pos, data.groundTile);
        } else {
            // Tile has no floor tile set, use dirt with the layer's color.
            this.groundMap.SetTile(pos, layerData.getFloorTile());
        }


        // Object Tile
        this.objectMap.GetTileFlags(pos);

        TileBase tile;
        Matrix4x4? matrix = null;
        if(data.rotationalOverride) {
            DirectionalTile dt = data.getObjectTile(state.rotation);
            tile = dt.tile;

            if(dt.effect != RotationEffect.NOTHING) {
                matrix = dt.getMatrix();
            }
        } else {
            tile = data.objectTile;
        }

        // If the Cell's behavior implements IRenderTileOverride, let it adject the tile, even if the tile is rotatable.
        if(state.hasBehavior() && state.behavior is IRenderTileOverride) {
            ((IRenderTileOverride)state.behavior).getObjectTile(ref tile);            
        }

        // Set the tile on the map.
        this.objectMap.SetTile(pos, tile);
        if(matrix != null) {
            this.objectMap.SetTransformMatrix(pos, (Matrix4x4)matrix);
        }


        // Overlay Tile
        this.overlayMap.SetTile(pos, data.overlayTile);

        // Color tiles:
        Color tint = layerData.getGroundTint();
        if(data.tintGroundTile || data.groundTile == null) { // Tint air
            this.func(this.groundMap, pos, tint);
        }
        if(data.tintObjectTile) {
            this.func(this.objectMap, pos, tint);
        }
    }

    private void func(Tilemap map, Vector3Int pos, Color c) {
        map.SetTileFlags(pos, TileFlags.None);
        map.SetColor(pos, c);
    }
}
