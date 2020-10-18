using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Tile", order = 1)]
public class BuildableTile : BuildableBase {

    [SerializeField]
    protected CellData _cell = null;

    public virtual CellData cell => this._cell;

    public override bool isRotatable() {
        return this.cell != null && this.cell.rotationalOverride;
    }

    public override bool isRotationValid(Rotation rotation) {
        if(this.cell == null) {
            return true;
        } else {
            return this.cell.isRotationOverrideEnabled(rotation);
        }
    }

    protected override void applyPreviewSprites(ref Sprite floorOverlaySprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        if(this._cell == null) {
            Debug.LogWarning("Can not display preview for BuildableTile " + this.name + ", it has no cell set");
            return;
        }

        TileRenderData dt = this._cell.getRenderData(Rotation.fromEnum(this.displayRotation));

        floorOverlaySprite = TileSpriteGetter.retrieveSprite(dt.floorOverlayTile);
        objectSprite = TileSpriteGetter.retrieveSprite(dt.objectTile);
        overlaySprite = TileSpriteGetter.retrieveSprite(dt.overlayTile);
    }

    public override bool isValidLocation(World world, Position pos, Rotation rotation) {
        if(!world.getCellState(pos).data.canBuildOver) {
            return false;
        }

        if(!CameraController.instance.inCreativeMode && !world.plotManager.isOwned(pos)) {
            return false;
        }

        return true;
    }

    public override void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        if(this.cell == null) {
            Debug.LogWarning("Can not place BuildableTile " + this.name + " into world, it has no Cell set.");
            return;
        }

        bool instantBuild = this.buildTime == 0 || CameraController.instance.inCreativeMode || highlight == null;

        if(instantBuild) {
            world.setCell(pos, this.cell, rotation);
        }
        else {
            world.setCell(pos, highlight.buildSiteCell, rotation);
            CellBehaviorBuildSite site = world.getBehavior<CellBehaviorBuildSite>(pos);
            site.setPrimary(this.cell, this.buildTime, this.fogOption == EnumFogOption.PLACE);
        }

        //this.applyFogOpperation(world, pos);
    }
}
