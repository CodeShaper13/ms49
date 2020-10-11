using fNbt;
using UnityEngine;

public class CellBehaviorRailLoadPoint : AbstractDepositPoint, IMinecartInteractor {

    public override bool isFull => this.minecart == null ? true : this.minecart.inventory.isFull();
    public override bool isEmpty => this.minecart == null ? false : this.minecart.inventory.isEmpty();
    public EntityMinecart minecart { get; set; }

    public override bool isOpen() {
        return this.minecart != null && !this.minecart.inventory.isFull();
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

    public override void deposit(Item item) {
        this.minecart.inventory.addItem(item);

        if(this.minecart.inventory.isFull()) {
            this.minecart.release();
            this.minecart = null;
        }
    }

    public bool shouldCartInteract(EntityMinecart cart) {
        return
            this.minecart == null && // There is no Minecart already here
            cart.position == this.pos &&
            !cart.inventory.isFull();
    }

    public Vector3 getCartStopPoint() {
        return this.center;
    }
}
