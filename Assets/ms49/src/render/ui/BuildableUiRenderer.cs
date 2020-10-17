using UnityEngine;
using UnityEngine.UI;

public class BuildableUiRenderer : MonoBehaviour {

    [SerializeField]
    private Image _imgFloorOverlay = null;
    [SerializeField]
    private Image _imgObject = null;
    [SerializeField]
    private Image _imgObjectOverlay = null;

    private void Awake() {
        this._imgFloorOverlay.enabled = false;
        this._imgObject.enabled = false;
        this._imgObjectOverlay.enabled = false;
    }

    public void setBuildable(BuildableBase buildable) {
        Sprite floorOverlaySprite = null;
        Sprite objectSprite = null;
        Sprite objectOverlaySprite = null;

        buildable.getSprites(ref floorOverlaySprite, ref objectSprite, ref objectOverlaySprite);

        this.func(floorOverlaySprite, this._imgFloorOverlay);
        this.func(objectSprite, this._imgObject);
        this.func(objectOverlaySprite, this._imgObjectOverlay);
    }

    private void func(Sprite sprite, Image img) {
        if(sprite == null) {
            img.enabled = false;
        }
        else {
            img.enabled = true;
            img.sprite = sprite;

            img.rectTransform.sizeDelta = sprite.rect.size;
            img.rectTransform.pivot = sprite.pivot / sprite.rect.size;
        }
    }
}
