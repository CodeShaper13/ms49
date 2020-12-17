using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupContainer : PopupWindow {

    [SerializeField]
    private Text headerText = null;
    [SerializeField]
    private Text emptyText = null;
    [SerializeField]
    private Text _itemCountText = null;
    [SerializeField]
    private RectTransform itemAreaTransform = null;
    [SerializeField]
    private GameObject inventoryItemEntryPrefab = null;

    private List<InventorySlot> slots;
    private Inventory inventory;

    protected override void initialize() {
        base.initialize();

        this.slots = new List<InventorySlot>();

        /*
        MinedItemRegistry registry = Main.instance.itemRegistry;
        for(int id = 0; id < registry.getRegistrySize(); id++) {
            Item item = registry.getElement(id);
            if(item != null) {
                GameObject obj = GameObject.Instantiate(this.inventoryItemEntryPrefab, this.itemAreaTransform);
                InventorySlot entry = obj.GetComponent<InventorySlot>();

                entry.setItem(item);

                this.slots.Add(entry);
            }
        }
        */
    }

    public void setInventory(Inventory newInventory) {
        // Destroy the old slot objects
        foreach(InventorySlot slot in this.slots) {
            GameObject.Destroy(slot.gameObject);
        }
        this.slots.Clear();
        this.headerText.text = "nul";


        if(newInventory != null) {
            this.inventory = newInventory;

            // Create slots.
            this.slots = new List<InventorySlot>();
            for(int i = 0; i < this.inventory.maxCapacity; i++) {
                GameObject obj = GameObject.Instantiate(this.inventoryItemEntryPrefab, this.itemAreaTransform);
                InventorySlot entry = obj.GetComponent<InventorySlot>();
                this.slots.Add(entry);
            }

            // Set head to state the inventory's name.
            this.headerText.text = string.IsNullOrEmpty(this.inventory.inventoryName) ? "inventory" : this.inventory.inventoryName.ToLower();
        }
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.inventory != null) {
            for(int i = 0; i < this.inventory.maxCapacity; i++) {
                InventorySlot slot = this.slots[i];
                Item item = this.inventory.getItem(i);

                if(item == null) {
                    slot.gameObject.SetActive(false);
                } else {
                    slot.gameObject.SetActive(true);
                    slot.setItem(item);
                }
                //if(slot.item != this.inventory.getItem(i)) {
                //    slot.setItem(this.inventory.getItem(i));
                //}
            }

            /*
            this.emptyText.enabled = this.inventory.isEmpty();

            foreach(InventorySlot entry in this.slots) {
                if(!this.inventory.isEmpty()) {
                    int count = 0;

                    foreach(Item item in this.inventory.items) {
                        if(item == entry.item) {
                            count++;
                        }
                    }

                    entry.setCount(count);
                    entry.gameObject.SetActive(count != 0);
                } else {
                    entry.gameObject.SetActive(false);
                }
            }
            */

            this._itemCountText.text =
                this.inventory.getItemCount() + "/" + this.inventory.maxCapacity + " items";
        }
    }

    protected override void onClose() {
        base.onClose();

        this.setInventory(null);
    }
}
