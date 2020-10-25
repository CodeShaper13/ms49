using UnityEngine;
using UnityEngine.UI;

public class EmoteBubble : MonoBehaviour {

    [SerializeField]
    private Image _emoteIconImg = null;
    [SerializeField]
    private Tooltip _tooltip = null;

    private float timer;
    private Emote currentEmote;

    private void Awake() {
        this.setBubbleVisible(false);
    }

    private void Update() {
        if(!Pause.isPaused()) {
            if(this.timer > 0) {
                this.timer -= Time.deltaTime;            

                if(this.timer <= 0) {
                    this.cancelEmote();
                }
            }
        }
    }

    public void startEmote(Emote emote) {
        if(this.currentEmote != null && (this.currentEmote.isPriority && !emote.isPriority)) {
            return;
        }

        this.currentEmote = emote;

        this.timer = emote.timeVisible;

        if(emote.sprite == null ) {
            Debug.LogWarning("It is not recomended to pass null for an emote sprite");
        }
        this._emoteIconImg.sprite = emote.sprite;

        if(!string.IsNullOrEmpty(emote.tooltip)) {
            this._tooltip.text = emote.tooltip;
        }

        this.setBubbleVisible(true);
    }

    public void cancelEmote() {
        this._tooltip.text = string.Empty;
        this.setBubbleVisible(false);
        this.currentEmote = null;
    }

    private void setBubbleVisible(bool visible) {
        this._emoteIconImg.gameObject.SetActive(visible);
    }
}
