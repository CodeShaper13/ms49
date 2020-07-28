using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string text;

    private TooltipDisplayer tooltip;

    private void Start() {
        this.tooltip = GameObject.FindObjectOfType<TooltipDisplayer>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        this.func(this.text);
    }

    public void OnPointerExit(PointerEventData eventData) {
        this.func(string.Empty);
    }

    private void func(string text) {
        if(this.tooltip != null) {
            this.tooltip.setText(text);
        }
    }
}
