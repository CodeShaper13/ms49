using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Rock", order = 1)]
public class BuildableRock : BuildableTile {

    [SerializeField]
    private Sprite previewSprite = null;

    public override void getPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        objectSprite = this.previewSprite;
    }
}
