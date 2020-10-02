using fNbt;

public class CellBehaviorRailSwitch : CellBehavior, IHasData, IRenderTileOverride, ILeverReciever {

    private bool isCurve;

    public void onLeverFlip(CellBehaviorLever lever) {
        this.isCurve = !this.isCurve;
        this.dirty();
    }

    public void readFromNbt(NbtCompound tag) {
        this.isCurve = tag.getBool("isSwitchCurved");
    }

    public void replaceTiles(ref TileRenderData renderData) {
        throw new System.NotImplementedException();
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("isSwitchCurved", this.isCurve);
    }
}
