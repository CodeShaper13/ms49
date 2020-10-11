public class CellBehaviorDepositPoint : AbstractDepositPoint {

    public override bool isOpen() {
        return !this.isFull;
    }
}
