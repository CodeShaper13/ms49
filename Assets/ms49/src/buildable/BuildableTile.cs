using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Buildable", menuName = "MS49/Buildable/Buildable Tile", order = 1)]
public class BuildableTile : BuildableBase {

    [SerializeField]
    protected CellData _cell = null;
    [SerializeField]
    private EnumFogOption _fogOption = EnumFogOption.LIFT;
    [SerializeField, HideInInspector, Tooltip("The rotation to display the tile with in the preview")]
    private EnumRotation _displayRotation = EnumRotation.UP;
    [SerializeField, HideInInspector, Tooltip("If not null or empty, this message will be used as the rotation msg")]
    private string overrideRotationMsg = null;

    public virtual CellData cell => this._cell;
    public virtual EnumRotation displayRotation => this._displayRotation;
    public EnumFogOption fogOption => this._fogOption;

    private void OnValidate() {
        if(this._displayRotation == EnumRotation.NONE) {
            Debug.Log("Display Rotation can not be NONE");
            this._displayRotation = EnumRotation.UP;
        }

        if(this.cell != null && !this.cell.isRotationOverrideEnabled(Rotation.fromEnum(this._displayRotation))) {
            Rotation r = Rotation.fromEnum(this.displayRotation);
            for(int i = 0; i < 4; i++) {
                if(this.cell.isRotationOverrideEnabled(r)) {
                    Debug.Log(this._displayRotation + " is not a valid rotation");
                    this._displayRotation = r.enumRot;
                    break;
                }

                r = r.clockwise();
            }
        }
    }

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

    public override string getRotationMsg() {
        return string.IsNullOrWhiteSpace(this.overrideRotationMsg) ? "rotate with r and shift + r" : this.overrideRotationMsg;
    }

    public virtual CellData getPreviewCell() {
        return this._cell;
    }

    public override void getPreviewSprites(ref Sprite floorOverlaySprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        CellData previewCell = this.getPreviewCell();

        if(previewCell == null) {
            Debug.LogWarning("Can not display preview for BuildableTile " + this.name + ", it has no cell set");
            return;
        }

        TileRenderData dt = previewCell.getRenderData(Rotation.fromEnum(this.displayRotation));

        floorOverlaySprite = TileSpriteGetter.retrieveSprite(dt.floorOverlayTile);
        objectSprite = TileSpriteGetter.retrieveSprite(dt.objectTile);
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

        this.applyFogOpperation(world, pos);
    }

    protected virtual void applyFogOpperation(World world, Position pos) {
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

#if UNITY_EDITOR
    [CustomEditor(typeof(BuildableTile), true)]
    public class BuildableTileEditor : Editor {

        private SerializedProperty displayRotation;
        private SerializedProperty overrideRotationMsg;

        private void OnEnable() {
            this.displayRotation = this.serializedObject.FindProperty("_displayRotation");
            this.overrideRotationMsg = this.serializedObject.FindProperty("overrideRotationMsg");
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            this.DrawDefaultInspector();

            BuildableTile script = (BuildableTile)this.target;

            if(script.cell != null && script.cell.rotationalOverride) {
                EditorGUILayout.PropertyField(this.displayRotation);
                EditorGUILayout.PropertyField(this.overrideRotationMsg);
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
