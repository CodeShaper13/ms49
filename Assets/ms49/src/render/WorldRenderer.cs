using System;
using UnityEngine;
using UnityEngine.Events;

public class WorldRenderer : MonoBehaviour {

    [SerializeField]
    private CellTilemapRenderer cellRenderer = null;
    [SerializeField]
    private FogRenderer fogRenderer = null;
    [SerializeField]
    private TargetedSquareTilemapRenderer _targetedRenderer = null;
    [SerializeField]
    private Color[] _hardnessColors = new Color[0];

    private Layer targetLayer;
    private World world;

    public int targetDepth => this.targetLayer == null ? -1 : this.targetLayer.depth;
    public bool initialized => this.world != null;

    public void startup(World world) {
        this.world = world;

        int size = this.world.mapSize;

        this.cellRenderer.initializedRenderer(
            size,
            (x, y) => {
                return this.world.mapGenerator.getLayerFromDepth(this.targetLayer.depth).getGroundTint(world, x, y);
            },
            (x, y) => {
                LayerData layerData = this.world.mapGenerator.getLayerFromDepth(this.targetLayer.depth);
                int hardness = this.targetLayer.getHardness(x, y);
                hardness = Mathf.Clamp(hardness, 0, this._hardnessColors.Length - 1);
                return this._hardnessColors[hardness] * layerData.getGroundTint(world, x, y);
            },
            (x, y) => {
                return this.targetLayer.getCellState(x, y);
            },
            (x, y) => {
                return this.world.mapGenerator.getLayerFromDepth(this.targetLayer.depth).getGroundTile(world, x, y);
            }
            );

        this.fogRenderer.mapSize = size;
    }

    public void shutdown() {
        this.world = null;
        this.targetLayer = null;

        this.cellRenderer.clear();
        this.fogRenderer.clear();
        this._targetedRenderer.clear();
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

    public void dirtyExcavationTarget(Position pos, TargetedSquare ts) {
        if(!this.initialized) {
            return;
        }

        if(this.targetLayer != null && pos.depth == this.targetLayer.depth) {
            this._targetedRenderer.setTile(pos.x, pos.y, ts);
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

        this._targetedRenderer.clear();
        foreach(TargetedSquare ts in this.world.targetedSquares.list) {
            this.dirtyExcavationTarget(ts.pos, ts);
        }
    }
}
