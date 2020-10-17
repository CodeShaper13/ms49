using System.Collections.Generic;
using UnityEngine;

public class PopupOreValues : PopupWorldReference {

    [SerializeField]
    private GameObject _valueBarPrefab = null;
    [SerializeField]
    private RectTransform _barParent = null;
    [SerializeField]
    private int _minBarHeight = 500;

    private List<ValueBar> bars;

    protected override void initialize() {
        base.initialize();

        this.bars = new List<ValueBar>();
    }

    protected override void onOpen() {
        base.onOpen();

        this.createBars();
    }

    protected override void onClose() {
        base.onClose();

        // Destroy all of the Bars
        foreach(Transform t in this._barParent) {
            GameObject.Destroy(t.gameObject);
        }

        this.bars.Clear();
    }

    protected override void onUpdate() {
        base.onUpdate();

        int highestPrice = 0;

        foreach(ValueBar bar in this.bars) {
            int value = this.world.economy.getItemValue(bar.item);
            bar.updatePrice(value);

            if(value > highestPrice) {
                highestPrice = value;
            }
        }

        highestPrice = Mathf.Max(highestPrice, this._minBarHeight);

        foreach(ValueBar bar in this.bars) {
            bar.setMaxValue(highestPrice);
        }
    }

    private void createBars() {
        this.bars = new List<ValueBar>();

        foreach(Item item in this.world.economy.getUnlockedItems()) {
            // Add a bar, this ore has been unlocked
            ValueBar bar = GameObject.Instantiate(this._valueBarPrefab, this._barParent).GetComponent<ValueBar>();
            bar.setItem(item);
            this.bars.Add(bar);
        }        
    }
}
