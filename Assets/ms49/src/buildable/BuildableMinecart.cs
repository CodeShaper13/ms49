using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Minecart", order = 1)]
public class BuildableMinecart : BuildableBase {

    [SerializeField]
    private Sprite railSprite = null;
    [SerializeField]
    private Sprite minecartSprite = null;

    public override void getPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite) {
        groundSprite = this.railSprite;
        objectSprite = this.minecartSprite;
    }

    public override bool isValidLocation(World world, Position pos) {
        return !world.isOutOfBounds(pos) && world.getCellState(pos).data is CellDataRail;
    }

    public override void placeIntoWorld(World world, Position pos, Rotation rotation) {
        world.spawnEntity(pos, 2); // Minecart id.
    }
}
