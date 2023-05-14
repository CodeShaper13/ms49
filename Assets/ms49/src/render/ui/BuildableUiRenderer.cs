using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class BuildableUiRenderer : MonoBehaviour {

    [SerializeField, Required]
    private Image _imgFloorOverlay = null;
    [SerializeField, Required]
    private Image _imgObject = null;
    [SerializeField, Required]
    private Image _imgObjectOverlay = null;

    [Space]

    [SerializeField]
    private BuildableBase _buildable = null;

    public BuildableBase Buildable => this._buildable;

    private void OnValidate() {
        this.SetBuildable(this.Buildable);
    }

    private void Awake() {
        this._imgFloorOverlay.enabled = false;
        this._imgObject.enabled = false;
        this._imgObjectOverlay.enabled = false;
    }

    public void SetBuildable(BuildableBase buildable) {
        this._buildable = buildable;

        Sprite floorOverlaySprite = null;
        Sprite objectSprite = null;
        Sprite objectOverlaySprite = null;

        if(buildable != null) {
            buildable.getSprites(ref floorOverlaySprite, ref objectSprite, ref objectOverlaySprite);
        }

        this.ApplyImgToSprite(floorOverlaySprite, this._imgFloorOverlay);
        this.ApplyImgToSprite(objectSprite, this._imgObject);
        this.ApplyImgToSprite(objectOverlaySprite, this._imgObjectOverlay);
    }

    private void ApplyImgToSprite(Sprite sprite, Image img) {
        img.enabled = sprite != null;

        if(sprite != null) {
            img.sprite = sprite;

            img.rectTransform.sizeDelta = sprite.rect.size;
            img.rectTransform.pivot = sprite.pivot / sprite.rect.size;
        }
    }
}
