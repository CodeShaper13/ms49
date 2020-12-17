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

    public IStatistic stat { get; private set; }
    public EnumStatisticCategory category { get; private set; }

    private void OnEnable() {
        if(this.stat != null) {
            this.updateLabels();
        }
    }

    public void setStat(RegisteredStat stat) {
        this.stat = stat.stat;
        this.category = stat.category;

        this.updateLabels();
    }

    public void setColor(bool useBrightColor) {
        Color c = useBrightColor ? this._brightColor : this._darkColor;
        this._textName.color = c;
        this._textValue.color = c;
    }

    private void updateLabels() {
        this._textName.text = this.stat.displayName;
        this._textValue.text = this.stat.displayValue;
    }
}
