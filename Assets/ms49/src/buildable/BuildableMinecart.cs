using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Minecart", order = 1)]
public class BuildableMinecart : BuildableBase, ISpritePreview {

    [SerializeField]
    private Sprite railSprite = null;
    [SerializeField]
    private EntityMinecart.MinecartSprites _minecartSprites = null;

    public override bool isRotatable() {
        return true;
    }

    protected override void applyPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        groundSprite = this.railSprite;
        objectSprite = this._minecartSprites.sideEmpty;
        overlaySprite = this._minecartSprites.sideFull;
    }

    public override bool isValidLocation(World world, Position pos, Rotation rotation) {
        return !world.isOutOfBounds(pos) && world.getCellState(pos).data is CellDataRail && world.plotManager.isOwned(pos);
    }

    public override void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        EntityMinecart minecart = (EntityMinecart)world.entities.spawn(pos, 2); // Minecart id.
        Rotation trackRot = world.getCellState(pos).rotation;
        minecart.rotation = rotation.axis == EnumAxis.Y ? trackRot : trackRot.opposite();

        // Incrase stat.
        world.statManager.minecartsPlaced.increase(1);
    }

    public Sprite getPreviewSprite(World world, Position pos) {
        CellState state = world.getCellState(pos);
        return state.rotation.axis == EnumAxis.X ? this._minecartSprites.sideEmpty : this._minecartSprites.frontEmpty;
    }
}
