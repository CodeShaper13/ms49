using UnityEngine;

public class WorldRenderer : MonoBehaviour {

    [SerializeField]
    private World world = null;

    [SerializeField]
    private CellTilemapRenderer cellRenderer = null;
    [SerializeField]
    private BinaryTilemapRenderer fogRenderer = null;
    [SerializeField]
    private BinaryTilemapRenderer targetedRenderer = null;

    /// <summary> The Layer the renderer is rendering. </summary>
    private Layer targetLayer;

    private void Awake() {
        int size = this.world.storage.mapSize;

        this.cellRenderer.mapSize = size;
        this.cellRenderer.floorTintGetterFunc = (x, y) => {
            return this.world.mapGenData.getLayerFromDepth(this.targetLayer.depth).getGroundTint();
        };
        this.cellRenderer.cellStateGetterFunc = (x, y) => {
            return this.targetLayer.getCellState(x, y);
        };
        this.cellRenderer.fallbackFloorGetterFunc = (x, y) => {
            return this.world.mapGenData.getLayerFromDepth(this.targetLayer.depth).getFloorTile();
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

    private void LateUpdate() {
        // Only show Entities that at the depth being rendered.
        foreach(EntityBase e in this.world.entityList) {
            if(e.depth == this.targetLayer.depth) {
                e.gameObject.SetActive(true);
            } else {
                e.gameObject.SetActive(false);
            }
        }

        foreach(Particle particle in this.world.particles.list) {
            if(particle.depth == this.targetLayer.depth) {
                particle.gameObject.SetActive(true);
            }
            else {
                particle.gameObject.SetActive(false);
            }
        }
    }

    public void dirtyTile(int x, int y) {
        this.cellRenderer.dirtyTile(x, y);
    }

    public void dirtyFogmap(Position pos, bool visible) {
        if(pos.depth == this.targetLayer.depth) {
            this.fogRenderer.setTile(pos.x, pos.y, visible);
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

        this.fogRenderer.redraw((x, y) => {
            if(layer.hasFog()) {
                return layer.fog.isFogPresent(x, y);
            } else {
                return false;
            }
        });

        this.targetedRenderer.clear();
        foreach(Position p in this.world.storage.targetedForRemovalSquares) {
            this.dirtyExcavationTarget(p, true);
        }
    }
}
