using UnityEngine;
using System.Collections;
using fNbt;

public class InventoryInfinite : Inventory {

    public InventoryInfinite(string containerName) : base(containerName, 1) {

    }

    public override bool isFull() {
        return false;
    }

    public override bool addItem(Item item) {
        this.items.Push(item);
        return true;
    }
}
