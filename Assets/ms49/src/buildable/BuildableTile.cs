using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Tile", order = 1)]
public class BuildableTile : BuildableBase {

    [SerializeField]
    protected CellData _cell = null;
    [SerializeField]
    private EnumFogOption _fogOption = EnumFogOption.LIFT;
    [SerializeField, Tooltip("The rotation to display the tile with in the preview")]
    private EnumRotation _displayRotation = EnumRotation.UP;
    [SerializeField, Tooltip("If not null or empty, this message will be used as the rotation msg")]
    private string overrideRotationMsg = null;

    public virtual CellData cell => this._cell;
    public virtual EnumRotation displayRotation => this._displayRotation;
    public EnumFogOption fogOption => this._fogOption;

    public override bool isRotatable() {
        return this.cell != null && this.cell.rotationalOverride;
    }

    public override string getRotationMsg() {
        return string.IsNullOrWhiteSpace(this.overrideRotationMsg) ? "rotate with r and shift + r" : this.overrideRotationMsg;
    }

    public virtual CellData getPreviewCell() {
        return this._cell;
    }

    public override void getPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        CellData previewCell = this.getPreviewCell();

        if(previewCell == null) {
            Debug.LogWarning("Can not display preview for BuildableTile " + this.name + ", it has no cell set");
            return;
        }

        groundSprite = TileSpriteGetter.retrieveSprite(previewCell.groundTile);

        DirectionalTile dt = previewCell.getObjectTile(Rotation.fromEnum(this.displayRotation));

        objectSprite = TileSpriteGetter.retrieveSprite(dt.tile);
        overlaySprite = TileSpriteGetter.retrieveSprite(dt.overlayTile);
    }

    public override bool isValidLocation(World world, Position pos, Rotation rotation) {
        if(world.isOutOfBounds(pos)) {
            return false;
        }

        if(!world.getCellState(pos).data.canBuildOver) {
            return false;
        }

        if(!world.plotManager.isOwned(pos)) {
            return false;
        }

        return true;
    }

    public override void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        if(this.cell == null) {
            Debug.LogWarning("Can not place BuildableTile " + this.name + " into world, it has no Cell set.");
            return;
        }

        bool instantBuild = CameraController.instance.inCreativeMode || highlight == null;

        if(instantBuild) {
            world.setCell(pos, this.cell, rotation);
        }
        else {
            world.setCell(pos, highlight.buildSiteCell, rotation);
            CellBehaviorBuildSite site = world.getBehavior<CellBehaviorBuildSite>(pos);
            site.addCell(this.cell, pos);
            site.isPrimary = true;
        }

        if(this.fogOption == EnumFogOption.LIFT) {
            world.liftFog(pos);
        } else if(this.fogOption == EnumFogOption.PLACE) {
            world.placeFog(pos);
        }
    }

    public enum EnumFogOption {
        LIFT = 0,
        PLACE = 1,
        NOTHING = 2,
    }
}
