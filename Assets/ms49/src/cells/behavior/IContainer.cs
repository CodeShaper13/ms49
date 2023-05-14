/// <summary>
/// Cell Behaviors that have inventories should implement this so
/// conveyor belts can interact with them.
/// </summary>
public interface IContainer {

    bool IsFull { get; }

    bool IsEmpty { get; }

    /// <summary>
    /// An implementation should attempt to add the passed item,
    /// returning true if it could be added.
    /// </summary>
    bool Deposit(Item item);

    /// <summary>
    /// Pulls the first item out of the container.
    /// </summary>
    Item PullItem();
}
