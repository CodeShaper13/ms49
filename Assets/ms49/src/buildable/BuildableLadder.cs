using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Ladder", order = 1)]
public class BuildableLadder : BuildableBase {

    [SerializeField]
    private CellData ladderTop = null;
    [SerializeField]
    private CellData ladderBottom = null;

    public override void getPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        Rotation r = Rotation.UP;

        PopupBuild popup = GameObject.FindObjectOfType<PopupBuild>();
        if(popup != null && popup.rot != null) {
            r = popup.rot;
        }

        CellData cell = this.pointingDown(r) ? this.ladderTop : this.ladderBottom;
        objectSprite = TileSpriteGetter.retrieveSprite(cell.getObjectTile(Rotation.UP).tile);
    }

    public override bool isRotatable() {
        return true;
    }

    public override string getRotationMsg() {
        return "change direction with r";
    }

    public override bool isValidLocation(World world, Position pos, Rotation rotation) {
        int otherDepth = pos.depth + (this.pointingDown(rotation) ? 1 : -1);

        if(!CameraController.instance.inCreativeMode && !world.isDepthUnlocked(otherDepth)) {
            return false;
        }

        return this.okToBuild(world, pos, true) && this.okToBuild(world, pos.setDepth(otherDepth), false);
    }

    public override void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation) {
        if(this.pointingDown(rotation)) {
            // Top, them bottom below

            this.func(world, pos, this.ladderTop);
            this.func(world, pos.setDepth(pos.depth + 1), this.ladderBottom);
        } else {
            this.func(world, pos, this.ladderBottom);
            this.func(world, pos.setDepth(pos.depth - 1), this.ladderTop);
        }
    }

    private bool pointingDown(Rotation r) {
        return r.axis == EnumAxis.Y;
    }

    private bool okToBuild(World world, Position pos, bool mustBeClear) {
        if(world.isOutOfBounds(pos)) { // Handles if the layer is too high or low
            return false;
        }

        CellData data = world.getCellState(pos).data;
        return (data.canBuildOver || (mustBeClear ? false : data is CellDataMineable)) && world.plotManager.isOwned(pos);
    }

    private void func(World world, Position pos, CellData cell) {
        world.setCell(pos, cell);
        world.liftFog(pos);
    }
}
