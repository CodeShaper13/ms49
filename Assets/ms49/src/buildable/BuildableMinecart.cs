using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Minecart", order = 1)]
public class BuildableMinecart : BuildableBase {

    [SerializeField]
    private Sprite railSprite = null;
    [SerializeField]
    private Sprite minecartSprite = null;
    [SerializeField]
    private Sprite fillSprite = null;

    public override bool isRotatable() {
        return true;
    }

    protected override void applyPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        groundSprite = this.railSprite;
        objectSprite = this.minecartSprite;
        overlaySprite = this.fillSprite;
    }

    public override bool isValidLocation(World world, Position pos, Rotation rotation) {
        return !world.isOutOfBounds(pos) && world.getCellState(pos).data is CellDataRail && world.plotManager.isOwned(pos);
    }

    public override void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        EntityMinecart minecart = (EntityMinecart)world.entities.spawn(pos, 2); // Minecart id.
        Rotation trackRot = world.getCellState(pos).rotation;
        minecart.facing = rotation.axis == EnumAxis.Y ? trackRot : trackRot.opposite();
    }
}
