using fNbt;
using System.Text;
using UnityEngine;

public class CellBehaviorRailStop : CellBehavior, IMinecartInteractor, IHasData {

    public static Mode alwaysStop = new Mode(0, "Always stop");
    public static Mode neverStop = new Mode(1, "Never stop");
    public static Mode stopIfFull = new Mode(2, "Stop if full");
    public static Mode stopIfEmpty = new Mode(3, "Stop if empty");
    public static Mode[] allModes = new Mode[] {
        alwaysStop, neverStop, stopIfFull, stopIfEmpty
    };

    public static Mode GetModeFromId(int id) {
        id = Mathf.Clamp(id, 0, allModes.Length - 1);
        return allModes[id];
    }

    public EntityMinecart minecart { get; set; }
    public Mode mode { get; set; } = CellBehaviorRailStop.alwaysStop;

    private void Update() {
        if(this.minecart != null) {

        }
    }

    public override void OnRightClick() {
        base.OnRightClick();

        PopupRailStop popup = Main.instance.FindPopup<PopupRailStop>();
        if(popup != null) {
            popup.SetRailStop(this);
            popup.open();
        }
    }

    public override void GetDebugText(StringBuilder sb, string indent) {
        base.GetDebugText(sb, indent);
        sb.AppendLine(indent + "Mode: " + this.mode.ToString());
    }

    public override void OnBehaviorDestroy() {
        base.OnBehaviorDestroy();

        if(this.minecart != null) {
            this.minecart.ReleaseFromInteractor();
        }
    }

    public Vector3 GetCartStopPoint() {
        return this.center;
    }

    public bool ShouldCartInteract(EntityMinecart cart) {
        if(this.minecart != null || cart.Position != this.pos) {
            return false;
        }

        if(this.mode == CellBehaviorRailStop.alwaysStop) {
            return true;
        } else if(this.mode == CellBehaviorRailStop.neverStop) {
            return false;
        } else if(this.mode == CellBehaviorRailStop.stopIfFull) {
            return cart.Inventory.IsFull;
        } else if(this.mode == CellBehaviorRailStop.stopIfEmpty) {
            return cart.Inventory.IsEmpty;
        }

        // Should never get here.
        return false;
    }

    public void ReadFromNbt(NbtCompound tag) {
        this.mode = CellBehaviorRailStop.GetModeFromId(tag.GetInt("interactMode"));
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.SetTag("interactMode", this.mode.id);
    }

    public class Mode {

        public readonly int id;
        public readonly string displayName;

        public Mode(int id, string displayName) {
            this.id = id;
            this.displayName = displayName;
        }

        public override string ToString() {
            return string.Format("Mode(id={0}, displayName={1})", this.id, this.displayName);
        }
    }
}