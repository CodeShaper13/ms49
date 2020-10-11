using fNbt;

public class CellBehaviorMasterDepositPoint : AbstractDepositPoint {

    public override bool isFull => false;
    public override bool isEmpty => true;

    public override void onRightClick() {
        // Don't list the parent class open a Container Popup.
    }

    public override void deposit(Item item) {
        this.world.economy.sellItem(item);
    }

    public override bool isOpen() {
        return true;
    }
}
