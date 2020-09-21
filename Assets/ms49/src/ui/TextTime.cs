using UnityEngine;
using UnityEngine.UI;

public class TextTime : MonoBehaviour {

    [SerializeField]
    private Text _textTime = null;
    [SerializeField]
    private Tooltip _tooltip = null;

    private GameTime gameTime;

    private void Update() {
        if(this.gameTime == null) {
            this.gameTime = GameObject.FindObjectOfType<GameTime>();
        }

        if(gameTime != null) {
            if(this._textTime != null) {
                this._textTime.text = this.gameTime.getFormattedTime();
            }

            if(this._tooltip != null) {
                this._tooltip.text = this.gameTime.getFormattedTimeExact();
            }

        } else {
            Debug.LogWarning("Unable to find GameTime Component.  Time will not be displayed.");
        }
    }
}
