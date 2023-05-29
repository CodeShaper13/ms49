using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipPrompt : MonoBehaviour, ITooltipPrompt, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField, TextArea]
    private string _text;
    public bool overrideDelay = false;
    [Min(0), ShowIf(nameof(overrideDelay))]
    public float delay = 0f;
    [SerializeField, Tooltip("If true, the text field will be set to the value of the attached Text component, or a Text Component of a child")]
    public bool pullFromTextComponent = false;

    private TooltipDisplayer tooltipDisplayer;

    public string text {
        get => this._text;
        set { this._text = value; }
    }

    // ITooltipPrompt implementation.
    public string Text => this._text;
    public bool OverrideDelay => this.overrideDelay;
    public float Delay => this.delay;

    private void Start() {
        this.tooltipDisplayer = GameObject.FindObjectOfType<TooltipDisplayer>();
        if(this.tooltipDisplayer == null) {
            Debug.LogWarning("Could not find TooltipDisplayer Component.  Tooltip will not be visible.");
        }

        if(this.pullFromTextComponent) {
            if(this.TryGetComponent(out TMP_Text tmpText)) {
                this._text = tmpText.text;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(!string.IsNullOrWhiteSpace(this.text)) {
            this.tooltipDisplayer?.Show(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        this.tooltipDisplayer?.Hide(this);
    }
}
