using UnityEngine;

public class WorldRenderer : MonoBehaviour {

    [SerializeField]
    private World world = null;

    [SerializeField]
    private CellTilemapRenderer cellRenderer = null;
    [SerializeField]
    private FogRenderer fogRenderer = null;
    [SerializeField]
    private BinaryTilemapRenderer targetedRenderer = null;

    /// <summary> The Layer the renderer is rendering. </summary>
    public Layer targetLayer;

    public void setup() {
        int size = this.world.mapSize;

        this.cellRenderer.mapSize = size;
        this.cellRenderer.floorTintGetterFunc = (x, y) => {
            return this.world.mapGenData.getLayerFromDepth(this.targetLayer.depth).getGroundTint(x, y);
        };
        this.cellRenderer.cellStateGetterFunc = (x, y) => {
            return this.targetLayer.getCellState(x, y);
        };
        this.cellRenderer.fallbackFloorGetterFunc = (x, y) => {
            return this.world.mapGenData.getLayerFromDepth(this.targetLayer.depth).getGroundTile(x, y);
        };

        this.fogRenderer.mapSize = size;
        this.targetedRenderer.mapSize = size;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F4)) {
            this.cellRenderer.totalRedraw = true;
            Debug.Log("Redrawing Map");
        }
    }

    public void dirtyTile(int x, int y) {
        this.cellRenderer.dirtyTile(x, y);
    }

    public void dirtyFogmap(Position pos, bool visible) {
        if(this.targetLayer != null && pos.depth == this.targetLayer.depth) {
            this.fogRenderer.dirtyTile(pos.x, pos.y, visible);
        }
    }

    public void dirtyExcavationTarget(Position pos, bool highlighted) {
        if(pos.depth == this.targetLayer.depth) {
            this.targetedRenderer.setTile(pos.x, pos.y, highlighted);
        }
    }

    public void setLayer(Layer layer) {
        this.targetLayer = layer;

        // Clear everything
        this.cellRenderer.clear();
        this.cellRenderer.totalRedraw = true;

        this.fogRenderer.redraw(layer);

        this.targetedRenderer.clear();
        foreach(Position p in this.world.targetedSquares.list) {
            this.dirtyExcavationTarget(p, true);
        }
    }
}
