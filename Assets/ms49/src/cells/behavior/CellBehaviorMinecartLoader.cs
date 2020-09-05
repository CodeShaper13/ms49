using UnityEngine;
using fNbt;

public class CellBehaviorMinecartLoader : CellBehaviorContainer {

    [SerializeField]
    private float itemTransferSpeed = 1f;

    private float transferTimer;

    public bool isUnloader { get; private set; }
    public EntityMinecart minecart { get; set; }

    public override void onUpdate() {
        base.onUpdate();

        this.transferTimer -= Time.deltaTime;

        if(this.minecart != null) {
            if(this.isUnloader) {
                // Pull items from the cart
                if(this.minecart.inventory.isEmpty()) {
                    // Cart is empty, send it off
                    this.releaseCart();
                    return;
                }

                if(this.transferTimer <= 0) {
                    this.deposit(this.minecart.inventory.pullItem());
                    this.transferTimer = this.itemTransferSpeed;
                }

            } else {
                // Add items to the cart.
                if(this.minecart.inventory.isFull()) {
                    // Cart is full, send it off.
                    this.releaseCart();
                    return;
                }

                if(this.transferTimer <= 0) {
                    this.minecart.inventory.addItem(this.pullItem());
                    this.transferTimer = this.itemTransferSpeed;
                }
            }
        }
    }

    public override void onDestroy() {
        base.onDestroy();

        if(this.minecart != null) {
            this.releaseCart();
        }
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.transferTimer = tag.getFloat("transferTimer");
        this.isUnloader = tag.getBool("isUnloader");
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("transferTimer", this.transferTimer);
        tag.setTag("isUnloader", this.isUnloader);
    }

    private void releaseCart() {
        this.minecart.foundLoader = null;
        this.minecart = null;
    }
}
