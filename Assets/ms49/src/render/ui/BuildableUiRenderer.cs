using UnityEngine;
using UnityEngine.UI;

public class BuildableUiRenderer : MonoBehaviour {

    [SerializeField]
    private Image uiGroundImage = null;
    [SerializeField]
    private Image uiDetailImage = null;
    [SerializeField]
    private Image uiOverlayImage = null;

    private void Awake() {
        // Hide all of the Images.
        this.uiGroundImage.enabled = false;
        this.uiDetailImage.enabled = false;
        this.uiOverlayImage.enabled = false;
    }

    public void setBuildable(BuildableBase buildable) {
        Sprite groundSprite = null;
        Sprite objectSprite = null;
        Sprite overlaySprite = null;
        buildable.getPreviewSprites(ref groundSprite, ref objectSprite, ref overlaySprite);

        this.funcPreview(groundSprite, this.uiGroundImage);
        this.funcPreview(objectSprite, this.uiDetailImage);
        this.funcPreview(overlaySprite, this.uiOverlayImage);
    }

    private void funcPreview(Sprite sprite, Image image) {
        if(sprite == null) {
            image.enabled = false;
        }
        else {
            image.enabled = true;
            image.sprite = sprite;

            if(sprite.rect.height == 16) {
                // Normal 16x16
                image.transform.localScale = Vector3.one;
                image.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            } else {
                // Tall sprite
                image.transform.localScale = new Vector3(1, 2, 1);
                image.rectTransform.pivot = new Vector2(0.5f, 0f);
            }
        }
    }
}
