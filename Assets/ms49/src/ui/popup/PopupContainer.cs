using UnityEngine;
using System.Collections.Generic;
using TMPro;
using NaughtyAttributes;

public class PopupContainer : PopupWindow {

    [Space]

    [SerializeField, Tooltip("Optional")]
    private TMP_Text _textContainerName = null;
    [SerializeField]
    private bool _generateSlots = true;
    [SerializeField, ShowIf(nameof(_generateSlots))]
    private RectTransform _slotParent = null;
    [SerializeField, ShowIf(nameof(_generateSlots))]
    private GameObject _prefabInventorySlot = null;
    [SerializeField, ShowIf(nameof(__notGenerateSlots))]
    private List<InventorySlot> _slots = null;

    private List<InventorySlot> slots;
    private Inventory inventory;

    private void Awake() {
        this.slots = new List<InventorySlot>();
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.inventory != null) {
            for(int i = 0; i < this.inventory.Size; i++) {
                if(i < this.slots.Count) {
                    InventorySlot slot = this.slots[i];
                    Item item = this.inventory[i];
                    slot.SetItem(item);
                }
            }
        }
    }

    public void SetInventory(Inventory newInventory) {
        if(this._generateSlots) {
            // Destroy the old slot objects
            foreach(InventorySlot slot in this.slots) {
                if(slot.gameObject == null) {
                    continue;
                }

                GameObject.Destroy(slot.gameObject);
            }
        }

        this.slots.Clear();

        if(newInventory != null) {
            this.inventory = newInventory;

            if(this._textContainerName != null) {
                this._textContainerName.text = string.IsNullOrEmpty(this.inventory.InventoryName) ? "Inventory" : this.inventory.InventoryName;
            }

            if(this._generateSlots) {
                // Create slots.
                for(int i = 0; i < this.inventory.Size; i++) {
                    InventorySlot slot = GameObject.Instantiate(
                        this._prefabInventorySlot,
                        this._slotParent).GetComponent<InventorySlot>();
                    this.slots.Add(slot);
                }
            } else {
                this.slots.AddRange(this._slots);

                if(this.slots.Count != this.inventory.Size) {
                    Debug.LogWarning("Inventory Size does not match the number of InventorySlot components on child objects.");
                }
            }
        } else {
            if(this._textContainerName != null) {
                this._textContainerName.text = string.Empty;
            }
        }
    }

    protected override void onClose() {
        base.onClose();

        this.SetInventory(null);
    }

    public bool __notGenerateSlots => !this._generateSlots;
}
