using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TooltipDisplayer : MonoBehaviour {

    [SerializeField]
    private Text textHeldItemName = null;
    [SerializeField]
    private Image img = null;

    private Tooltip textsource;

    private void Start() {
        this.setText(string.Empty);
        this.textHeldItemName.text = string.Empty;
    }

    private void Update() {
        this.transform.position = Input.mousePosition;
    }

    public void setText(string text) { //, float delay, Tooltip source) {
        /*
        if(source == this.textsource) {
            // The tooltip currenlty being show is trying to update
            // it's text.  Let it happen instantly
            this.textHeldItemName.text = text;
        } else {
            // New tooltip, wait to show it's text.
            this.StartCoroutine(this.func(text, delay, source));
        }
        */

        if(this.img != null) {
            this.img.enabled = !string.IsNullOrEmpty(text);
        }
        if(this.textHeldItemName != null) {
            this.textHeldItemName.text = text;
        }
    }

    private IEnumerator func(string text, float delay, Tooltip source) {
        yield return new WaitForSecondsRealtime(delay);

        this.textsource = source;
        this.textHeldItemName.text = text;
    }
}
