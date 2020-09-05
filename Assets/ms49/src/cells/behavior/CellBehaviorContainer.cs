using UnityEngine;
using fNbt;
using System.Text;

public class CellBehaviorContainer : CellBehavior, IHasData, IContainer {

    [SerializeField, Min(0)]
    protected int inventorySize = 10;
    [SerializeField]
    protected string containerName = string.Empty;

    protected Inventory inventory;

    public virtual bool isFull => this.inventory.isFull();
    public virtual bool isEmpty => this.inventory.isEmpty();

    public override void onCreate(World world, CellState state, Position pos) {
        base.onCreate(world, state, pos);

        this.inventory = new Inventory(this.containerName, this.inventorySize);
    }

    public override void onRightClick() {
        base.onRightClick();

        PopupContainer popup = Main.instance.findPopup<PopupContainer>();
        if(popup != null) {
            popup.open();
            popup.setInventory(this.inventory);
        }
    }

    public virtual void writeToNbt(NbtCompound tag) {
        tag.setTag("inventory", this.inventory.writeToNbt());
    }

    public virtual void readFromNbt(NbtCompound tag) {
        this.inventory.readFromNbt(tag.getCompound("inventory"));
    }

    public virtual void deposit(Item item) {
        this.inventory.addItem(item);
    }

    public virtual Item pullItem() {
        return this.inventory.pullItem();
    }
}
