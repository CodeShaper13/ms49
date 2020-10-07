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

        CellBehaviorLever.alertNeighborsOfFlip(this);
    }

    public void readFromNbt(NbtCompound tag) {
        this.isOn = tag.getBool("isLeverOn");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("isLeverOn", this.isOn);
    }

    /// <summary>
    /// Alerts neighbors of a lever being flipped.
    /// </summary>
    public static void alertNeighborsOfFlip(CellBehavior leverBehavior) {
        foreach(Rotation r in Rotation.ALL) {
            Position p1 = leverBehavior.pos + r;
            CellBehavior behavior =
                leverBehavior.world.getBehavior<CellBehavior>(p1);
            if(behavior is ILeverReciever) {
                ((ILeverReciever)behavior).onLeverFlip(leverBehavior);
            }
        }
    }
}
