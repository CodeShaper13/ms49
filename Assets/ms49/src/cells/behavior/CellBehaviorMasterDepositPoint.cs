using fNbt;

public class CellBehaviorMasterDepositPoint : AbstractDepositPoint {

    public override bool IsFull => false;
    public override bool IsEmpty => true;

    public override void onRightClick() {
        // Don't let the parent class open a Container Popup.
    }

    public override bool Deposit(Item item) {
        this.world.economy.sellItem(item);
        return true;
    }

    public override bool isOpen() {
        return true;
    }
}
