
using fNbt;
using System.Collections.Generic;

public class Inventory {

    protected Stack<Item> items;
    protected int maxCapacity;

    public Inventory(int size) {
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
            ids[i] = reg.getIdOfElement(items.Pop());
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