using UnityEngine;
using UnityEngine.UI;

public class ValueBar : MonoBehaviour {

    [SerializeField]
    private Text _textValue = null;
    [SerializeField]
    private Text _textName = null;
    [SerializeField]
    private Slider _slider = null;
    [SerializeField]
    private Slider _bonusSlider = null;
    [SerializeField]
    private Image _image = null;
    [SerializeField]
    private Image _bonusFillImg = null;

    public Item item { get; private set; }

    public void setItem(Item item) {
        this.item = item;

        this._image.color = this.item.graphColor;
        this._bonusFillImg.color = new Color(
            this.item.graphColor.r,
            this.item.graphColor.g,
            this.item.graphColor.b,
            0.6f);

        this._textName.text = this.item.itemName;

        this._slider.value = item.moneyValue;
    }    

    public void updatePrice(int value) {
        this._textValue.text = "$" + value;
        this._bonusSlider.value = value;
    }

    public void setMaxValue(int max) {
        this._slider.maxValue = max;
        this._bonusSlider.maxValue = max;
    }
}
