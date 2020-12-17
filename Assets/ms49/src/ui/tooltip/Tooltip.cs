using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField, TextArea(1, 5)]
    private string _text;
    [SerializeField]
    private float delay = 0.5f;
    [SerializeField]
    [Tooltip("If true, the text field will be set to the value of the attached Text component, or a Text Component of a child")]
    public bool pullFromTextComponent;

    private TooltipDisplayer tooltipDisplayer;

    public string text {
        get {
            return this._text;
        }
        set {
            this._text = value;
        }
    }

    private void Start() {
        this.tooltipDisplayer = GameObject.FindObjectOfType<TooltipDisplayer>();
        if(this.tooltipDisplayer == null) {
            Debug.LogWarning("Could not find TooltipDisplayer Component.  Tooltip will not be visible.");
        }

        if(this.pullFromTextComponent) {
            Text textComponent = this.GetComponent<Text>();
            if(textComponent != null) {
                this._text = textComponent.text;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(this.tooltipDisplayer != null && !string.IsNullOrWhiteSpace(this.text)) {
            this.tooltipDisplayer.setText(this.text, this.delay, this.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(this.tooltipDisplayer != null && !string.IsNullOrWhiteSpace(this.text)) {
            this.tooltipDisplayer.hide();
        }
    }
}
