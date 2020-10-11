using UnityEngine;

public class CellBehaviorRailUnloadPoint : CellBehaviorOccupiable, IMinecartInteractor {

    public EntityMinecart minecart { get; set; }

    public override void onDestroy() {
        base.onDestroy();

        if(this.minecart != null) {
            this.minecart.release();
        }
    }

    public override void onUpdate() {
        base.onUpdate();

        if(this.minecart != null && this.minecart.inventory.isEmpty()) {
            this.minecart.release();
            this.minecart = null;
        }
    }

    public Vector3 getCartStopPoint() {
        return this.center;
    }

    public bool shouldCartInteract(EntityMinecart cart) {
        return
            this.minecart == null &&
            cart.position == this.pos &&
            !cart.inventory.isEmpty();
    }
}
