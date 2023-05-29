using UnityEngine;
using fNbt;

public class CellBehaviorMinecartLoader : CellBehaviorContainer, IMinecartInteractor {

    [SerializeField]
    private float _itemTransferSpeed = 1f;
    [SerializeField]
    private bool _isUnloader = false;

    private float transferTimer;

    public EntityMinecart minecart { get; set; }

    public bool isUnloader => this._isUnloader;

    private void Update() {
        if(Pause.IsPaused) {
            return;
        }

        this.transferTimer -= Time.deltaTime;

        if(this.minecart != null) {
            if(this.isUnloader) {
                // Pull items from the cart
                if(this.minecart.Inventory.IsEmpty || this.inventory.IsFull) {
                    // Cart is empty, send it off
                    this.releaseCart();
                    return;
                }

                if(this.transferTimer <= 0) {
                    this.Deposit(this.minecart.Inventory.PullItem());
                    this.transferTimer = this._itemTransferSpeed;
                }

            } else {
                // Add items to the cart.
                if(this.minecart.Inventory.IsFull || this.inventory.IsEmpty) {
                    // Cart is full, send it off.
                    this.releaseCart();
                    return;
                }

                if(this.transferTimer <= 0) {
                    this.minecart.Inventory.AddItem(this.PullItem());
                    this.transferTimer = this._itemTransferSpeed;
                }
            }
        }
    }

    public override void OnBehaviorDestroy() {
        base.OnBehaviorDestroy();

        if(this.minecart != null) {
            this.releaseCart();
        }
    }

    public override void ReadFromNbt(NbtCompound tag) {
        base.ReadFromNbt(tag);

        this.transferTimer = tag.GetFloat("transferTimer");
    }

    public override void WriteToNbt(NbtCompound tag) {
        base.WriteToNbt(tag);

        tag.SetTag("transferTimer", this.transferTimer);
    }

    private void releaseCart() {
        this.minecart.ReleaseFromInteractor();
        this.minecart = null;
    }

    public bool ShouldCartInteract(EntityMinecart cart) {
        if(cart.Position != this.pos + this.rotation) {
            return false; // Minecart not in front of Loader.
        }

        if(this.isUnloader) {
            return !this.IsFull && !cart.Inventory.IsEmpty;
        }
        else {
            return !this.IsEmpty && !cart.Inventory.IsFull;
        }
    }

    public Vector3 GetCartStopPoint() {
        return this.center + this.rotation.vectorF;
    }
}
