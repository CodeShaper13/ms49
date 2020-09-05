using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupContainer : PopupWindow {

    [SerializeField]
    private Text headerText = null;
    [SerializeField]
    private Text emptyText = null;
    [SerializeField]
    private RectTransform itemAreaTransform = null;
    [SerializeField]
    private GameObject inventoryItemEntryPrefab = null;

    private List<InventoryItemEntry> inventoryEntries;
    private Inventory inventory;

    protected override void initialize() {
        base.initialize();

        this.inventoryEntries = new List<InventoryItemEntry>();

        MinedItemRegistry registry = Main.instance.itemRegistry;
        for(int id = 0; id < registry.getRegistrySize(); id++) {
            Item item = registry.getElement(id);
            if(item != null) {
                GameObject obj = GameObject.Instantiate(this.inventoryItemEntryPrefab, this.itemAreaTransform);
                InventoryItemEntry entry = obj.GetComponent<InventoryItemEntry>();

                entry.setItem(item);

                this.inventoryEntries.Add(entry);
            }
        }
    }

    public void setInventory(Inventory inventory) {
        this.inventory = inventory;

        this.headerText.text = string.IsNullOrEmpty(this.inventory.inventoryName) ? "inventory" : this.inventory.inventoryName.ToLower();
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.inventory != null) {
            this.emptyText.enabled = this.inventory.isEmpty();

            foreach(InventoryItemEntry entry in this.inventoryEntries) {
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
        }
    }

    protected override void onClose() {
        base.onClose();

        this.inventory = null;
    }
}
