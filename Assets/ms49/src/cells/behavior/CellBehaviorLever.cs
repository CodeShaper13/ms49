using fNbt;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellBehaviorLever : CellBehavior, IHasData, IRenderTileOverride {

    [SerializeField]
    private TileBase onTile = null;

    private bool isOn;

    public void replaceTiles(ref TileBase floorOverlay, ref TileBase tile, ref TileBase tileOverlay) {
        if(this.isOn) {
            tile = this.onTile;
        }
    }

    public override void onRightClick() {
        base.onRightClick();

        this.isOn = !this.isOn;
        this.dirty();
    }

    public void readFromNbt(NbtCompound tag) {
        this.isOn = tag.getBool("isLeverOn");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("isLeverOn", this.isOn);
    }
}
