using UnityEngine;
using UnityEngine.UI;

public class UiStatisticEntry : MonoBehaviour {

    [SerializeField]
    private Text _textName = null;
    [SerializeField]
    private Text _textValue = null;
    [SerializeField]
    private Color _brightColor = Color.white;
    [SerializeField]
    private Color _darkColor = Color.gray;

    private IStatistic stat;

    public void setStat(IStatistic stat, bool useBrightColor) {
        this.stat = stat;

        Color c = useBrightColor ? this._brightColor : this._darkColor;
        this._textName.color = c;
        this._textValue.color = c;

        this.updateLabels();
    }

    private void OnEnable() {
        if(this.stat != null) {
            this.updateLabels();
        }
    }

    private void updateLabels() {
        this._textName.text = this.stat.displayName;
        this._textValue.text = this.stat.displayValue;
    }
}
