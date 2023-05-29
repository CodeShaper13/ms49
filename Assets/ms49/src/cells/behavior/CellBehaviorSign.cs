using fNbt;

public class CellBehaviorSign : CellBehavior, IHasData {

    public string message { get; set; }

    public override void OnRightClick() {
        base.OnRightClick();

        PopupSign popup = Main.instance.FindPopup<PopupSign>();
        if(popup != null) {
            popup.open();
            popup.setMeta(this);
        }
    }

    public override string GetTooltipText() {
        return "[rbm] edit sign";
    }

    public void WriteToNbt(NbtCompound nbt) {
        nbt.SetTag("message", this.message);
    }

    public void ReadFromNbt(NbtCompound nbt) {
        this.message = nbt.GetString("message");
    }
}
