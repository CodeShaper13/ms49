using UnityEngine;

public class CellBehaviorRailUnloadPoint : CellBehaviorOccupiable, IMinecartInteractor {

    public EntityMinecart minecart { get; set; }

    public override void onDestroy() {
        base.onDestroy();

        if(this.minecart != null) {
            this.minecart.release();
        }
    }

    private void Update() {
        if(this.minecart != null && this.minecart.Inventory.IsEmpty) {
            this.minecart.release();
            this.minecart = null;
        }
    }

    public Vector3 GetCartStopPoint() {
        return this.center;
    }

    public bool ShouldCartInteract(EntityMinecart cart) {
        return
            this.minecart == null &&
            cart.position == this.pos &&
            !cart.Inventory.IsEmpty;
    }
}
