using fNbt;

public class CellBehaviorMasterDepositPoint : AbstractDepositPoint {

    public override bool isFull => false;
    public override bool isEmpty => true;

    public override void onRightClick() {
        // Don't list the parent class open a Container Popup.
    }

    public override bool deposit(Item item) {
        this.world.economy.sellItem(item);
        return true;
    }

    public override bool isOpen() {
        return true;
    }
}
