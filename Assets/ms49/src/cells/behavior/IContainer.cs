﻿/// <summary>
/// Cell Behaviors that have inventories should implement this so
/// conveyor belts can interact with them.
/// </summary>
public interface IContainer {

    bool isFull { get; }

    bool isEmpty { get; }

    void deposit(Item item);

    Item pullItem();
}
