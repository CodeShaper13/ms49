public class CellBehaviorResearchTable : CellBehavior, IContainer {

    public bool IsFull => false;

    public bool IsEmpty => true;

    public bool Deposit(Item item) {
        return this.world.technologyTree.AddResearch(item);
    }

    public Item PullItem() {
        return null;
    }
}