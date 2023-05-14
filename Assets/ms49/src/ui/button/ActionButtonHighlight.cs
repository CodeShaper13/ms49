using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ActionButtonHighlight : MonoBehaviour {

    [SerializeField]
    private PopupWindow _popup = null;

    private Image btnImage;
    private Sprite normalSprite;
    private Sprite highlightSprite;

    private void Awake() {
        Button button = this.GetComponent<Button>();

        this.btnImage = button.image;
        this.normalSprite = this.btnImage.sprite;
        this.highlightSprite = button.spriteState.highlightedSprite;
    }

    private void Update() {
        if(this._popup != null) {
            this.btnImage.sprite = this._popup.IsOpen ? this.highlightSprite : this.normalSprite;
        }
    }
}
