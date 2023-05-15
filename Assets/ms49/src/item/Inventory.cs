using fNbt;
using UnityEngine;

public class Inventory : MonoBehaviour {

    [SerializeField]
    private string _inventoryName = "";
    [SerializeField, Min(1)]
    private int _size = 1;

    private Item[] items;

    public string InventoryName => this._inventoryName;
    public int Size => this._size;

    private void Awake() {
        this.items = new Item[this.Size];
    }

    public virtual bool IsFull => this.GetItemCount() >= this.Size;

    public virtual bool IsEmpty => this.GetItemCount() == 0;

    /// <summary>
    /// Returns the number of Items in the Inventory.
    /// </summary>
    /// <returns></returns>
    public int GetItemCount() {
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
    public virtual bool AddItem(Item item) {
        if(item == null) {
            Debug.LogWarning("Can't add null to an Inventory");
            return true;
        }

        if(!this.IsFull) {
            this.items[this.GetItemCount()] = item;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes and returns an Item from the Inventory.
    /// If the Inventory is empty, null is returned.
    /// </summary>
    public virtual Item PullItem() {
        if(this.IsEmpty) {
            return null;
        } else {
            int index = this.GetItemCount() - 1;
            Item item = this.items[index];
            this.items[index] = null;
            return item;
        }
    }

    public virtual NbtCompound WriteToNbt() {
        NbtCompound tag = new NbtCompound();

        ItemRegistry reg = Main.instance.ItemRegistry;
        int count = this.items.Length;
        int[] ids = new int[count];
        for(int i = 0; i < count; i++) {
            Item item = this.items[i];
            ids[i] = item == null ? -1 : reg.GetIdOfElement(item);
        }
        tag.setTag("itemIds", ids);

        return tag;
    }

    public virtual void ReadFromNbt(NbtCompound tag) {
        ItemRegistry registry = Main.instance.ItemRegistry;
        int[] ids = tag.getIntArray("itemIds");
        for(int i = 0; i < ids.Length; i++) {
            int id = ids[i];
            this.items[i] = id == -1 ? null : registry[ids[i]];
        }
    }

    public Item this[int index] {
        get => this.items[index];
        set => this.items[index] = value;
    }
}