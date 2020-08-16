using UnityEngine;
using fNbt;

public class CellBehaviorDepositPoint : CellBehavior, IHasData {

    [SerializeField, Tooltip("Master points do not store items.  Instead they \"sell\" the items.")]
    private bool isMaster = false;
    [SerializeField]
    private int size = 10;

    private Inventory inventory;

    private void OnValidate() {
        if(this.isMaster) {
            this.size = 0;
        }
    }

    public override void onCreate(World world,CellState state, Position pos) {
        base.onCreate(world, state, pos);

        this.inventory = new Inventory(this.size);
    }

    public override void onUpdate() {
        base.onUpdate();

        if(!this.inventory.isEmpty()) {
            // Search for conveyor belts nearby that lead away.
            foreach(Rotation r in Rotation.ALL) {
                CellBehaviorConveyorBelt belt = this.world.getBehavior<CellBehaviorConveyorBelt>(this.pos + r);
                if(belt != null && belt.rotation == r) {
                    belt.addItem(this.inventory.pullItem());
                }

                if(this.inventory.isEmpty()) {
                    break;
                }
            }
        }
    }

    public Item deposit(Item item) {
        if(this.isMaster) {
            this.world.money.value += item.moneyValue;
            return null;
        } else {
            if(this.inventory.isFull()) {
                return item;
            } else {
                this.inventory.addItem(item);
                return null;
            }
        }
    }

    public void writeToNbt(NbtCompound tag) {
        if(!this.isMaster) {
            tag.setTag("inventory", this.inventory.writeToNbt());
        }
    }

    public void readFromNbt(NbtCompound tag) {
        if(!this.isMaster) {
            this.inventory.readFromNbt(tag.getCompound("inventory"));
        }
    }
}
