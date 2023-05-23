using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipDisplayer : MonoBehaviour {

    [SerializeField, Required]
    private RectTransform _tooltipFrame = null;
    [SerializeField, Required]
    private TMP_Text _text = null;
    [SerializeField, Min(0f)]
    private float _revealDelay = 0.25f;

    private ITooltipPrompt prompt;
    private float timer;
    private Vector3 mousePos;

    private float Delay => this.prompt != null && this.prompt.OverrideDelay ? this.prompt.Delay : this._revealDelay;

    private void Awake() {
        this.Hide(null);
    }

    private void LateUpdate() {
        if(this.prompt != null) {
            if(this.mousePos != Input.mousePosition) {
                this.mousePos = Input.mousePosition;
                this.timer = this.Delay;
            }

            if(this.timer > 0) {
                this.timer -= Time.deltaTime;
            }
            else {
                if(!this._tooltipFrame.gameObject.activeSelf) {
                    // First frame the tooltip is visible.
                    this._tooltipFrame.gameObject.SetActive(true);
                }

                string promptText = this.prompt.Text;
                if(promptText != this._text.text) {
                    this._text.text = promptText;
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(this._tooltipFrame);
            }
        }

        this._tooltipFrame.position =  Input.mousePosition;
    }

    public void Show(ITooltipPrompt prompt) {
        if(prompt == this.prompt) {
            return;
        }

        this.prompt = prompt;
        this.timer = this.Delay;
        this.mousePos = Input.mousePosition;
    }

    public void Hide(ITooltipPrompt prompt) {
        //if(this.prompt != prompt) {
            // Somehow a different tooltip is being shown, so don't do anything.
        //    return;
        //}

        this.prompt = null;
        this._tooltipFrame.gameObject.SetActive(false);
    }
}