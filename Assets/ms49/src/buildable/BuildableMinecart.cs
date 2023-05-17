using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Minecart", order = 1)]
public class BuildableMinecart : BuildableBase, ISpritePreview {

    [Space]

    [SerializeField]
    private Sprite railSprite = null;
    [SerializeField, Required]
    private GameObject _minecartPrefab = null;

    public override bool IsRotatable() {
        return true;
    }

    protected override void applyPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        groundSprite = this.railSprite;

        EntityMinecart.MinecartSprites sprites = this._minecartPrefab.GetComponent<EntityMinecart>().minecartSprites;
        objectSprite = sprites.GetSprite(Rotation.RIGHT, true);
        overlaySprite = sprites.GetSprite(Rotation.RIGHT, false);
    }

    public override bool IsValidLocation(World world, Position pos, Rotation rotation) {
        return !world.IsOutOfBounds(pos) && world.GetCellState(pos).data is CellDataRail && world.plotManager.IsOwned(pos);
    }

    public override void PlaceIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        EntityMinecart minecart = (EntityMinecart)world.entities.Spawn(pos, Main.instance.EntityRegistry.GetIdOfElement(this._minecartPrefab));
        Rotation trackRot = world.GetCellState(pos).Rotation;
        minecart.rotation = rotation.axis == EnumAxis.Y ? trackRot : trackRot.opposite();

        // Incrase stat.
        world.statManager.minecartsPlaced.increase(1);
    }

    public Sprite GetPreviewSprite(World world, Position pos) {
        CellState state = world.GetCellState(pos);
        return this._minecartPrefab.GetComponent<EntityMinecart>().minecartSprites.GetSprite(state.Rotation, true);
    }
}
