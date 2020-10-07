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

    public int cost => this._cost;
    public string description => this._description;
    public Tab tab => this._tab;
    public EnumFogOption fogOption => this._fogOption;
    public EnumRotation displayRotation => this._displayRotation;

    private void OnValidate() {
        if(this._displayRotation == EnumRotation.NONE) {
            Debug.Log("Display Rotation can not be NONE");
            this._displayRotation = EnumRotation.UP;
        }

        if(!this.isRotationValid(Rotation.fromEnum(this._displayRotation))) {
            Rotation r = Rotation.fromEnum(this.displayRotation);
            for(int i = 0; i < 4; i++) {
                if(this.isRotationValid(r)) {
                    Debug.Log(this.displayRotation + " is not a valid rotation");
                    this._displayRotation = r.enumRot;
                    break;
                }

                r = r.clockwise();
            }
        }
    }

    public virtual string getName() {
        return this.structureName;
    }

    /// <summary>
    /// If true is returned, the buildable is considered "rotatable"
    /// and it's state can be changed with r and shift + r.
    /// </summary>
    public virtual bool isRotatable() {
        return false;
    }

    public virtual bool isRotationValid(Rotation rotation) {
        return true;
    }

    /// <summary>
    /// If this Buildable is rotatable (BuildableBase#isRotatable
    /// returns true) the rotate tip message uses the returned text.
    /// </summary>
    public virtual string getRotationMsg() {
        return string.IsNullOrWhiteSpace(this._overrideRotationMsg) ? "[r] to rotate" : this._overrideRotationMsg;
    }

    public virtual int getHighlightWidth() {
        return 1;
    }

    public virtual int getHighlightHeight() {
        return 1;
    }

    public abstract void getPreviewSprites(
        ref Sprite groundSprite,
        ref Sprite objectSprite,
        ref Sprite overlaySprite);

    /// <summary>
    /// Places the structure into the world.  highlight is null if a Structure is placing this Buildable.
    /// </summary>
    public abstract void placeIntoWorld(World world, BuildAreaHighlighter highlight, Position pos, Rotation rotation);

    /// <summary>
    /// Returns true if the Structure can go at the passed position.
    /// </summary>
    public abstract bool isValidLocation(World world, Position pos, Rotation rotation);

    protected void applyFogOpperation(World world, Position pos) {
        if(this.fogOption == EnumFogOption.LIFT) {
            world.liftFog(pos);
        }
        else if(this.fogOption == EnumFogOption.PLACE) {
            world.placeFog(pos);
        }
    }

    public enum EnumFogOption {
        LIFT = 0,
        PLACE = 1,
        NOTHING = 2,
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BuildableBase), true)]
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

            if(script.isRotatable()) {
                EditorGUILayout.PropertyField(this.displayRotation);
                EditorGUILayout.PropertyField(this.overrideRotationMsg);
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
