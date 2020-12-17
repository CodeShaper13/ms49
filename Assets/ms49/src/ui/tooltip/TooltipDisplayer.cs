using UnityEngine;
using UnityEngine.UI;

public class TooltipDisplayer : MonoBehaviour {

    [SerializeField]
    private Text _textElement = null;
    [SerializeField]
    private Image _backgroundImg = null;

    private float tooltipShowDelay;
    private string tooltipText;

    public GameObject tooltipSource { get; private set; }

    private void Start() {
        this.hide();
    }

    private void Update() {
        // Make the tooltip follow the mouse and correct the background size
        this.transform.position = Input.mousePosition;
        this._backgroundImg.rectTransform.sizeDelta =
            this._textElement.rectTransform.sizeDelta + new Vector2(6, 4);

        if(this.tooltipSource != null) {

            if(this.tooltipShowDelay <= 0) {
                this._textElement.text = this.tooltipText;
                this.show();
            }
            else {
                this.tooltipShowDelay -= Time.unscaledDeltaTime;
            }

        } else {
            this.hide();
        }
    }

    public void setText(string text, float delay, GameObject source) {
        if(source != this.tooltipSource) {
            // Reset timer, a new object is requesting a tooltip
            this.hide();

            this.tooltipSource = source;
            this.tooltipShowDelay = delay;
        }

        this.tooltipText = text;
    }

    private void show() {
        this._textElement.enabled = true;
        this._backgroundImg.enabled = true;
    }

    public void hide() {
        this.tooltipSource = null;
        this._textElement.enabled = false;
        this._backgroundImg.enabled = false;
    }
}
