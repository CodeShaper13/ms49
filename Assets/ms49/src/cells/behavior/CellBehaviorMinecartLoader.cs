﻿using UnityEngine;
using fNbt;

public class CellBehaviorMinecartLoader : AbstractBehaviorContainer, IMinecartInteractor {

    [SerializeField]
    private float _itemTransferSpeed = 1f;
    [SerializeField]
    private bool _isUnloader = false;

    private float transferTimer;

    public EntityMinecart minecart { get; set; }

    public bool isUnloader => this._isUnloader;

    public override void onUpdate() {
        base.onUpdate();

        this.transferTimer -= Time.deltaTime;

        if(this.minecart != null) {
            if(this.isUnloader) {
                // Pull items from the cart
                if(this.minecart.inventory.isEmpty() || this.inventory.isFull()) {
                    // Cart is empty, send it off
                    this.releaseCart();
                    return;
                }

                if(this.transferTimer <= 0) {
                    this.deposit(this.minecart.inventory.pullItem());
                    this.transferTimer = this._itemTransferSpeed;
                }

            } else {
                // Add items to the cart.
                if(this.minecart.inventory.isFull() || this.inventory.isEmpty()) {
                    // Cart is full, send it off.
                    this.releaseCart();
                    return;
                }

                if(this.transferTimer <= 0) {
                    this.minecart.inventory.addItem(this.pullItem());
                    this.transferTimer = this._itemTransferSpeed;
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
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("transferTimer", this.transferTimer);
    }

    private void releaseCart() {
        this.minecart.release();
        this.minecart = null;
    }

    public bool shouldCartInteract(EntityMinecart cart) {
        if(cart.position != this.pos + this.rotation) {
            return false; // Minecart not in front of Loader.
        }

        if(this.isUnloader) {
            return !this.isFull && !cart.inventory.isEmpty();
        }
        else {
            return !this.isEmpty && !cart.inventory.isFull();
        }
    }

    public Vector3 getCartStopPoint() {
        return this.center + this.rotation.vectorF;
    }
}
