using fNbt;

public class CellBehaviorSign : CellBehavior, IHasData {

    public string message { get; set; }

    public override void onRightClick() {
        base.onRightClick();

        PopupSign popup = Main.instance.FindPopup<PopupSign>();
        if(popup != null) {
            popup.open();
            popup.setMeta(this);
        }
    }

    public override string getTooltipText() {
        return "[rbm] edit sign";
    }

    public void WriteToNbt(NbtCompound nbt) {
        nbt.setTag("message", this.message);
    }

    public void ReadFromNbt(NbtCompound nbt) {
        this.message = nbt.getString("message");
    }
}
