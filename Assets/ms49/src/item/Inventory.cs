using fNbt;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory {

    protected int maxCapacity;

    public Stack<Item> items { get; protected set; }
    public string inventoryName { get; protected set; }

    public Inventory(string inventoryName, int size) {
        this.inventoryName = inventoryName;
        this.maxCapacity = size;
        this.items = new Stack<Item>(this.maxCapacity);
    }

    public virtual bool isFull() {
        return this.items.Count >= this.maxCapacity;
    }

    public virtual bool isEmpty() {
        return this.items.Count == 0;
    }

    /// <summary>
    /// Adds an item to the Minecart if there is space.  True is
    /// returned if the item is added, false if it is not.
    /// </summary>
    public virtual bool addItem(Item item) {
        if(item == null) {
            Debug.LogWarning("Can't add null to an Inventory");
            return true;
        }

        if(!this.isFull()) {
            this.items.Push(item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes and returns an Item from the inventory.
    /// If the inventory is empty, null is returned.
    /// </summary>
    public virtual Item pullItem() {
        if(this.isEmpty()) {
            return null;
        } else {
            return this.items.Pop();
        }
    }

    public virtual NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        MinedItemRegistry reg = Main.instance.itemRegistry;
        int count = this.items.Count;
        int[] ids = new int[count];
        for(int i = 0; i < count; i++) {
            ids[i] = reg.getIdOfElement(items.ElementAt(i));
        }
        tag.setTag("itemIds", ids);

        return tag;
    }

    public virtual void readFromNbt(NbtCompound tag) {
        MinedItemRegistry reg = Main.instance.itemRegistry;
        int[] ids = tag.getIntArray("itemIds");
        for(int i = 0; i < ids.Length; i++) {
            Item item = reg.getElement(ids[i]);
            if(item != null) {
                this.items.Push(item);
            }
        }
    }
}