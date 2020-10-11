using UnityEngine;
using fNbt;

public abstract class CellBehaviorContainer : CellBehavior, IHasData, IContainer {

    [SerializeField]
    private Inventory _inventory = null;

    protected Inventory inventory => this._inventory;
    public virtual bool isFull => this.inventory.isFull();
    public virtual bool isEmpty => this.inventory.isEmpty();

    public override void onCreate(World world, CellState state, Position pos) {
        base.onCreate(world, state, pos);
    }

    public override void onRightClick() {
        base.onRightClick();

        PopupContainer popup = Main.instance.findPopup<PopupContainer>();
        if(popup != null) {
            popup.open();
            popup.setInventory(this.inventory);
        } else {
            Debug.LogWarning("Could not find PopupContainer");
        }
    }

    public virtual void writeToNbt(NbtCompound tag) {
        if(this.inventory != null) {
            tag.setTag("inventory", this.inventory.writeToNbt());
        }
    }

    public virtual void readFromNbt(NbtCompound tag) {
        if(this.inventory != null) {
            this.inventory.readFromNbt(tag.getCompound("inventory"));
        }
    }

    public virtual void deposit(Item item) {
        this.inventory.addItem(item);
    }

    public virtual Item pullItem() {
        return this.inventory.pullItem();
    }
}
