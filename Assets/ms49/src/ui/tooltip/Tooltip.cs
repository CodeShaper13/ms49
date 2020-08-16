using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField]
    private string _text;
    [SerializeField]
    private float delay = 0.5f;
    [SerializeField]
    [Tooltip("If true, the text field will be set to the value of the attached Text component, or a Text Component of a child")]
    public bool pullFromTextComponent;

    public string text {
        get {
            return this._text;
        }
        set {
            this._text = value;
        }
    }

    // Cached
    private TooltipDisplayer tooltip;

    private void Start() {
        this.tooltip = GameObject.FindObjectOfType<TooltipDisplayer>();

        if(this.pullFromTextComponent) {
            Text textComponent = this.GetComponent<Text>();
            if(textComponent != null) {
                this._text = textComponent.text;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(this.tooltip != null) {
            this.Invoke("updateTooltip", this.delay);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(this.tooltip != null) {
            this.tooltip.setText(string.Empty);
            this.CancelInvoke("updateTooltip");
        }
    }

    private void updateTooltip() {
        if(this.tooltip != null) {
            this.tooltip.setText(this.text);
        }
    }
}
