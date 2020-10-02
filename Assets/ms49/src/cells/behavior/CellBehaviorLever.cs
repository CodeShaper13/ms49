using fNbt;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellBehaviorLever : CellBehavior, IHasData, IRenderTileOverride {

    [SerializeField]
    private TileBase onTile = null;

    private bool isOn;

    public void replaceTiles(ref TileRenderData renderData) {
        if(this.isOn) {
            renderData.objectTile = this.onTile;
        }
    }

    public override void onRightClick() {
        base.onRightClick();

        this.isOn = !this.isOn;
        this.dirty();

        // Alert neightbors of the lever being flipped.
        foreach(Rotation r in Rotation.ALL) {
            CellBehavior behavior = this.world.getBehavior<CellBehavior>(this.pos + r);
            if(behavior is ILeverReciever) {
                ((ILeverReciever)behavior).onLeverFlip(this);
            }
        }
    }

    public void readFromNbt(NbtCompound tag) {
        this.isOn = tag.getBool("isLeverOn");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("isLeverOn", this.isOn);
    }
}
