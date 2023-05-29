public class CellBehaviorMasterDepositPoint : AbstractDepositPoint {

    public override bool IsFull => false;
    public override bool IsEmpty => true;

    public EntityTruck truck;

    public override void OnRightClick() {
        // Don't let the parent class open a Container Popup.
    }

    public override bool Deposit(Item item) {
        if(this.truck != null) {
            this.truck.Inventory.AddItem(item);
            return true;
        } else {
            return false;
        }
    }

    public override bool IsAcceptingItems() {
        return true;
    }
}
