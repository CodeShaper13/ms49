using UnityEngine;
using UnityEngine.Tilemaps;

public class CellBehaviorSink : CellBehaviorOccupiable, IRenderTileOverride {

    [SerializeField]
    private TileBase tileSinkFilled = null;

    private bool isFilled;

    public void replaceTiles(ref TileBase floorOverlay, ref TileBase tile, ref TileBase tileOverlay) {
        if(this.isFilled) {
            tile = this.tileSinkFilled;
        }
    }

    public void setFilled(bool filled) {
        if(this.isFilled != filled) {
            this.dirty();
        }

        this.isFilled = filled;
    }
}
