public class CellBehaviorDepositPoint : AbstractDepositPoint {

    public override bool IsAcceptingItems() {
        return !this.IsFull;
    }
}
