using UnityEngine;
using UnityEngine.UI;

public class OverlayGemValueBar : MonoBehaviour {

    [SerializeField]
    private TooltipPrompt _toltip = null;
    [SerializeField]
    private Slider _slider = null;
    [SerializeField]
    private Image _foregroundImage = null;
    [SerializeField]
    private Image _backgroundImgage = null;

    public Item item { get; private set; }

    public void setItem(Item item) {
        this.item = item;

        this._foregroundImage.color = this.item.graphColor;
        this._backgroundImgage.color = this.item.darkGraphColor;
        this._toltip.text = this.item.itemName;
    }

    public void update(Economy economy) {
        // Tooltip update:
        this._toltip.text =
            this.item.itemName +
            " $" +
            economy.getItemValue(this.item);

        // Slider Bar update:
        int baseValue = economy.getItemValueUnmodified(item);
        int currentValue = economy.getItemValue(item);
        int maxValue = economy.getItemMaxValue(item);

        this._slider.minValue = baseValue - (maxValue - baseValue);
        this._slider.maxValue = maxValue;
        this._slider.value = currentValue;
    }
}
