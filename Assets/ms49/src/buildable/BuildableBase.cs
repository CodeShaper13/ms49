using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public abstract class BuildableBase : ScriptableObject {

    [SerializeField]
    private string structureName = string.Empty;
    [SerializeField, Min(0)]
    private int _cost = 0;
    [SerializeField, TextArea(3, 10)]
    private string _description = string.Empty;
    [SerializeField, Tooltip("If blank, this will be put in the miscellaneous tab.")]
    private Tab _tab = null;
    [SerializeField]
    private EnumFogOption _fogOption = EnumFogOption.LIFT;
    [SerializeField, HideInInspector, Tooltip("If not null or empty, this message will be used as the rotation msg")]
    private string _overrideRotationMsg = null;
    [SerializeField, HideInInspector, Tooltip("The rotation to display the tile with in the preview")]
    private EnumRotation _displayRotation = EnumRotation.UP;
    [SerializeField, Min(0), Tooltip("If 0, this Buildable is constructed instantly.")]
    private float _buildTime = 0;
    [SerializeField, Tooltip("If set, this sprite will be using in the build popup.")]
    private Sprite _customPreview = null;

    public int cost => this._cost;
    public string description => this._description;
    public Tab Tab => this._tab;
    public EnumFogOption FogOption => this._fogOption;
    public EnumRotation displayRotation => this._displayRotation;
    public float BuildTime => this._buildTime;
    public Sprite customPreviewSprite => this._customPreview;

    private void OnValidate() {
        if(this._displayRotation == EnumRotation.NONE) {
            Debug.LogWarning("Display Rotation can not be NONE");
            this._displayRotation = EnumRotation.UP;
        }

        if(!this.IsRotationValid(Rotation.fromEnum(this._displayRotation))) {
            Rotation r = Rotation.fromEnum(this.displayRotation);
            for(int i = 0; i < 4; i++) {
                if(this.IsRotationValid(r)) {
                    Debug.Log(this._displayRotation + " is not a valid rotation");
                    this._displayRotation = r.enumRot;
                    break;
                }

                r = r.clockwise();
            }
        }
    }

    /// <summary>
    /// Returns the name of the Buildable.  This is shown in the GUI.
    /// </summary>
    public virtual string GetBuildableName() {
        return this.structureName;
    }

    /// <summary>
    /// If true is returned, the buildable is considered "rotatable"
    /// and it's state can be changed with r and shift + r.
    /// </summary>
    public virtual bool IsRotatable() {
        return false;
    }

    /// <summary>
    /// Checks if the Buildable allows it to be placed at hte passed
    /// rotation.  Some Buildable may only be allowed to be place in
    /// 2 or 3 directions.
    /// </summary>
    public virtual bool IsRotationValid(Rotation rotation) {
        return true;
    }

    /// <summary>
    /// If this Buildable is rotatable (BuildableBase#isRotatable
    /// returns true) the rotate tip message uses the returned text.
    /// </summary>
    public virtual string GetRotationMsg() {
        return string.IsNullOrWhiteSpace(this._overrideRotationMsg) ? "[r] to rotate" : this._overrideRotationMsg;
    }

    public virtual int GetBuildableWidth() {
        return 1;
    }

    public virtual int GetBuildableHeight() {
        return 1;
    }

    public void getSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) {
        if(this._customPreview != null) {
            objectSprite = this._customPreview;
        } else {
            this.applyPreviewSprites(ref groundSprite, ref objectSprite, ref overlaySprite);
        }
    }

    protected virtual void applyPreviewSprites(ref Sprite groundSprite, ref Sprite objectSprite, ref Sprite overlaySprite) { }

    /// <summary>
    /// Places the Buildable into the world.  Highlight is null if a Structure is placing this Buildable during world generation.
    /// </summary>
    public abstract void PlaceIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation);

    /// <summary>
    /// Returns true if the Structure can go at the passed position.
    /// </summary>
    public abstract bool IsValidLocation(World world, Position pos, Rotation rotation);

    protected void ApplyFogOpperation(World world, Position pos) {
        if(this.FogOption == EnumFogOption.LIFT) {
            world.LiftFog(pos);
        }
        else if(this.FogOption == EnumFogOption.PLACE) {
            world.PlaceFog(pos);
        }
    }

    public enum EnumFogOption {
        LIFT = 0,
        PLACE = 1,
        NOTHING = 2,
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BuildableBase), true)]
    [CanEditMultipleObjects]
    public class BuildableBaseEditor : Editor {

        private SerializedProperty displayRotation;
        private SerializedProperty overrideRotationMsg;

        protected virtual void OnEnable() {
            this.displayRotation = this.serializedObject.FindProperty("_displayRotation");
            this.overrideRotationMsg = this.serializedObject.FindProperty("_overrideRotationMsg");
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            this.DrawDefaultInspector();

            BuildableBase script = (BuildableBase)this.target;

            if(script.IsRotatable()) {
                EditorGUILayout.PropertyField(this.displayRotation);
                EditorGUILayout.PropertyField(this.overrideRotationMsg);
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
