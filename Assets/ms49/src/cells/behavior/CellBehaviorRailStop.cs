using fNbt;
using UnityEngine;

public class CellBehaviorRailStop : CellBehavior, IMinecartInteractor, IHasData {

    public static Mode alwaysStop = new Mode(0, "Always stop");
    public static Mode neverStop = new Mode(0, "Never stop");
    public static Mode stopIfFull = new Mode(0, "Stop if full");
    public static Mode stopIfEmpty = new Mode(0, "Stop if empty");
    public static Mode[] allModes = new Mode[] {
        alwaysStop, neverStop, stopIfFull, stopIfEmpty
    };

    public static Mode GetModeFromId(int id) {
        id = Mathf.Clamp(id, 0, allModes.Length - 1);
        return allModes[id];
    }

    public EntityMinecart minecart { get; set; }
    public Mode mode { get; set; } = CellBehaviorRailStop.alwaysStop;

    public override void onUpdate() {
        base.onUpdate();

        if(this.minecart != null) {

        }
    }

    public override void onRightClick() {
        base.onRightClick();

        PopupRailStop popup = Main.instance.findPopup<PopupRailStop>();
        if(popup != null) {
            popup.SetRailStop(this);
            popup.open();
        }
    }

    public override void onDestroy() {
        base.onDestroy();

        if(this.minecart != null) {
            this.minecart.release();
        }
    }

    public Vector3 GetCartStopPoint() {
        return this.center;
    }

    public bool ShouldCartInteract(EntityMinecart cart) {
        if(this.minecart != null || cart.position != this.pos) {
            return false;
        }

        if(this.mode == CellBehaviorRailStop.alwaysStop) {
            return true;
        } else if(this.mode == CellBehaviorRailStop.neverStop) {
            return false;
        } else if(this.mode == CellBehaviorRailStop.stopIfFull) {
            return cart.inventory.IsFull;
        } else if(this.mode == CellBehaviorRailStop.stopIfEmpty) {
            return cart.inventory.IsEmpty;
        }

        // Should never get here.
        return false;
    }

    public void ReadFromNbt(NbtCompound tag) {
        this.mode = CellBehaviorRailStop.GetModeFromId(tag.getInt("interactMode"));
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.setTag("interactMode", this.mode.id);
    }

    public class Mode {

        public int id;
        public string displayName;

        public Mode(int id, string displayName) {
            this.id = id;
            this.displayName = displayName;
        }
    }
}