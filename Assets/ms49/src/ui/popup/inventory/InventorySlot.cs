using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    [SerializeField]
    private Text count = null;
    [SerializeField]
    private Image img = null;
    [SerializeField]
    private Tooltip tooltip = null;

    public Item item { get; private set; }

    public void setItem(Item item) {
        this.item = item;

        if(item == null) {
            this.img.sprite = null;
        } else {
            this.img.sprite = item.sprite;
        }

        if(this.tooltip != null) {
            this.tooltip.text = this.item.itemName;
        }
    }

    public void setCount(int count) {
        this.count.text = "x" + count;
    }
}
