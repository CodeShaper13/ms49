using fNbt;

public class CellBehaviorSign : CellBehavior, IHasData {

    public string message { get; set; }

    public override void onRightClick() {
        base.onRightClick();

        PopupSign popup = Main.instance.findPopup<PopupSign>();
        if(popup != null) {
            popup.open();
            popup.setMeta(this);
        }
    }

    public void writeToNbt(NbtCompound nbt) {
        nbt.setTag("message", this.message);
    }

    public void readFromNbt(NbtCompound nbt) {
        this.message = nbt.getString("message");
    }
}
