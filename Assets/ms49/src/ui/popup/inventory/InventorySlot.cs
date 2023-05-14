using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    [SerializeField]
    private TMP_Text count = null;
    [SerializeField]
    private Image img = null;
    [SerializeField]
    private Tooltip tooltip = null;

    [Space]

    [SerializeField]
    private Item item;

    public Item Item { get; private set; }

    private void OnValidate() {
        this.SetItem(this.item);
    }

    public void SetItem(Item item) {
        this.item = item;

        if(item == null) {
            this.img.enabled = false;
        } else {
            this.img.enabled = true;
            this.img.sprite = item.sprite;
        }

        if(this.tooltip != null) {
            this.tooltip.text = item != null ? item.itemName : string.Empty;
        }
    }

    /*
    public void SetCount(int count) {
        this.count.text = "x" + count;
    }
    */
}
