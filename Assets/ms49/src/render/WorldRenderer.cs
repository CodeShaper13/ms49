using UnityEngine;

public class WorldRenderer : MonoBehaviour {

    [SerializeField]
    private CellTilemapRenderer cellRenderer = null;
    [SerializeField]
    private FogRenderer fogRenderer = null;
    [SerializeField]
    private BinaryTilemapRenderer targetedRenderer = null;

    private Layer targetLayer;
    private World world;

    public int targetDepth => this.targetLayer == null ? -1 : this.targetLayer.depth;
    public bool initialized => this.world != null;

    public void startup(World world) {
        this.world = world;

        int size = this.world.mapSize;

        this.cellRenderer.initializedRenderer(
            size,
            (x, y) => { return this.world.mapGenData.getLayerFromDepth(this.targetLayer.depth).getGroundTint(x, y); },
            (x, y) => { return this.targetLayer.getCellState(x, y); },
            (x, y) => { return this.world.mapGenData.getLayerFromDepth(this.targetLayer.depth).getGroundTile(x, y); }
            );

        this.fogRenderer.mapSize = size;
        this.targetedRenderer.mapSize = size;
    }

    public void shutdown() {
        this.world = null;
        this.targetLayer = null;

        this.cellRenderer.clear();
        this.fogRenderer.clear();
        this.targetedRenderer.clear();
    }

    private void Update() {
        if(!this.initialized) {
            return;
        }

        if(Input.GetKeyDown(KeyCode.F4)) {
            this.cellRenderer.totalRedraw = true;
            Debug.Log("Redrawing Map");
        }
    }

    public void dirtyTile(int x, int y) {
        if(!this.initialized) {
            return;
        }

        this.cellRenderer.dirtyCell(x, y);
    }

    public void dirtyFogmap(Position pos, bool visible) {
        if(!this.initialized) {
            return;
        }

        if(this.targetLayer != null && pos.depth == this.targetLayer.depth) {
            this.fogRenderer.dirtyTile(pos.x, pos.y, visible);
        }
    }

    public void dirtyExcavationTarget(Position pos, bool highlighted) {
        if(!this.initialized) {
            return;
        }

        if(this.targetLayer != null && pos.depth == this.targetLayer.depth) {
            this.targetedRenderer.setTile(pos.x, pos.y, highlighted);
        }
    }

    /// <summary>
    /// Returns the depth of the Layer being rendered.  If no layer
    /// is set, -1 is returned.
    /// </summary>
    public int getDepthRendering() {
        if(this.initialized) {
            if(this.targetLayer != null) {
                return this.targetLayer.depth;
            }
        }

        return -1;
    }

    public void setLayer(Layer layer) {
        if(!this.initialized) {
            return;
        }

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
