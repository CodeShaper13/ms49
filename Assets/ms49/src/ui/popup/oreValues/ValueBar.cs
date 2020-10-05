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
    private Image _image = null;

    public Item item { get; private set; }

    public void setItem(Item item) {
        this.item = item;

        this._image.color = this.item.graphColor;
        this._textName.text = this.item.itemName;
    }    

    public void setSliderValue(int value) {
        this._textValue.text = "$" + value;
        this._slider.value = value;
    }
}
