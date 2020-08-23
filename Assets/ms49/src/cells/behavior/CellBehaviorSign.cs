using fNbt;
using UnityEngine;

public class CellBehaviorSign : CellBehavior, IHasData {

    public string message { get; set; }

    public override void onRightClick() {
        base.onRightClick();

        PopupSign popup = GameObject.FindObjectOfType<PopupSign>();
        if(popup != null) {
            popup.open();
            popup.setMeta(this);
        } else {
            Debug.Log("Popup could not be found!");
        }
    }

    public void writeToNbt(NbtCompound nbt) {
        nbt.setTag("message", this.message);
    }

    public void readFromNbt(NbtCompound nbt) {
        this.message = nbt.getString("message");
    }
}
