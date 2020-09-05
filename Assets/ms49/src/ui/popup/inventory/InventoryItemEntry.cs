using UnityEngine;
using UnityEngine.UI;

public class InventoryItemEntry : MonoBehaviour {

    [SerializeField]
    private Text count = null;
    [SerializeField]
    private Image img = null;

    public Item item { get; private set; }

    public void setItem(Item item) {
        this.item = item;

        if(item == null) {
            this.img.sprite = null;
        } else {
            this.img.sprite = item.sprite;
        }
    }

    public void setCount(int count) {
        this.count.text = "x" + count;
    }
}
