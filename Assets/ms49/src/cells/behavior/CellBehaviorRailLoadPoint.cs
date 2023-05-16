using UnityEngine;

public class CellBehaviorRailLoadPoint : AbstractDepositPoint, IMinecartInteractor {

    public override bool IsFull => this.minecart == null ? true : this.minecart.Inventory.IsFull;
    public override bool IsEmpty => this.minecart == null ? false : this.minecart.Inventory.IsEmpty;
    public EntityMinecart minecart { get; set; }

    public override bool isOpen() {
        return this.minecart != null && !this.minecart.Inventory.IsFull;
    }

    public override void onRightClick() {
        // Don't show container
    }

    public override void onDestroy() {
        base.onDestroy();

        if(this.minecart != null) {
            this.minecart.release();
        }
    }

    public override bool Deposit(Item item) {
        bool addedItem = this.minecart.Inventory.AddItem(item);

        if(this.minecart.Inventory.IsFull) {
            this.minecart.release();
            this.minecart = null;
        }

        return addedItem;
    }

    public bool ShouldCartInteract(EntityMinecart cart) {
        return
            this.minecart == null && // There is no Minecart already here
            cart.position == this.pos &&
            !cart.Inventory.IsFull;
    }

    public Vector3 GetCartStopPoint() {
        return this.center;
    }
}
