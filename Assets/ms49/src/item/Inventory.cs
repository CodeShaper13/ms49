using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour {

    [SerializeField]
    private string _inventoryName = "";
    [SerializeField, Min(1)]
    private int _size = 1;

    private Item[] items;

    public string inventoryName => this._inventoryName;
    public int maxCapacity => this._size;

    private void Awake() {
        this.items = new Item[this.maxCapacity];
    }

    public virtual bool isFull() {
        return this.getItemCount() >= this.maxCapacity;
    }

    public virtual bool isEmpty() {
        return this.getItemCount() == 0;
    }

    public int getItemCount() {
        int count = 0;
        for(int i = 0; i < this.items.Length; i++) {
            if(this.items[i] != null) {
                count++;
            }
        }
        return count;
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
            this.items[this.getItemCount()] = item;
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
            int index = this.getItemCount() - 1;
            Item item = this.items[index];
            this.items[index] = null;
            return item;
        }
    }

    public virtual NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        MinedItemRegistry reg = Main.instance.itemRegistry;
        int count = this.items.Length;
        int[] ids = new int[count];
        for(int i = 0; i < count; i++) {
            ids[i] = reg.getIdOfElement(items[i]);
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
                this.items[i] = item;
            }
        }
    }

    public Item getItem(int i) {
        return this.items[i];
    }
}