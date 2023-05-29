using UnityEngine;

public class CellBehaviorRailUnloadPoint : CellBehaviorOccupiable, IMinecartInteractor {

    public EntityMinecart minecart { get; set; }

    public override void OnBehaviorDestroy() {
        base.OnBehaviorDestroy();

        if(this.minecart != null) {
            this.minecart.ReleaseFromInteractor();
        }
    }

    private void Update() {
        if(this.minecart != null && this.minecart.Inventory.IsEmpty) {
            this.minecart.ReleaseFromInteractor();
            this.minecart = null;
        }
    }

    public Vector3 GetCartStopPoint() {
        return this.center;
    }

    public bool ShouldCartInteract(EntityMinecart cart) {
        return
            this.minecart == null &&
            cart.Position == this.pos &&
            !cart.Inventory.IsEmpty;
    }
}
