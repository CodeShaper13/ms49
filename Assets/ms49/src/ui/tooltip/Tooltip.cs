using System.Collections;
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
        if(this.tooltip == null) {
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
        this.StartCoroutine("func");
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(this.tooltip != null) {
            this.tooltip.setText(string.Empty);
            this.StopCoroutine("func");
        }
    }

    private IEnumerator func() {
        yield return new WaitForSecondsRealtime(this.delay);
        if(this.tooltip != null) {
            this.tooltip.setText(this.text);
        }
    }
}
