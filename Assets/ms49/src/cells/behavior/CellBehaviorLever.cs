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

    public override void OnRightClick() {
        base.OnRightClick();

        this.isOn = !this.isOn;
        this.dirty();

        this.world.statManager.leversFlipped.increase(1);

        CellBehaviorLever.alertNeighborsOfFlip(this);
    }

    public override string GetTooltipText() {
        return "[rmb] " + (this.isOn ? "Turn On" : "Turn Off");
    }

    public void ReadFromNbt(NbtCompound tag) {
        this.isOn = tag.GetBool("isLeverOn");
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.SetTag("isLeverOn", this.isOn);
    }

    /// <summary>
    /// Alerts neighbors of a lever being flipped.
    /// </summary>
    public static void alertNeighborsOfFlip(CellBehavior leverBehavior) {
        foreach(Rotation r in Rotation.ALL) {
            Position p1 = leverBehavior.pos + r;
            CellBehavior behavior = leverBehavior.world.GetCellBehavior<CellBehavior>(p1, true);
            if(behavior is ILeverReciever leverReciever) {
                leverReciever.OnLeverFlip(leverBehavior);
            }
        }
    }
}
