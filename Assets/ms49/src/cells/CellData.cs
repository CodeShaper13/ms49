using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Cell", menuName = "MS49/Cell/Cell", order = 1)]
public class CellData : ScriptableObject {

    [SerializeField, Tooltip("If true, the floor is not drawn below and an edge will surround this tile.")]
    private bool _isSolid = false;
    [SerializeField]
    private TileBase _floorOverlayTile = null;
    [SerializeField]
    private TileBase _objectTile = null;
    [SerializeField]
    private TileBase _objectOverlayTile = null;

    [SerializeField]
    private bool _rotationalOverride = false;

    [SerializeField, HideInInspector]
    private RotationOverride up = null;
    [SerializeField, HideInInspector]
    private RotationOverride right = null;
    [SerializeField, HideInInspector]
    private RotationOverride down = null;
    [SerializeField, HideInInspector]
    private RotationOverride left = null;

    [Space]

    [SerializeField, Tooltip("-1 = not walkable, 0 = walkable, greater than 1 is walkable with a penalty"), Min(-1)]
    private int _movementCost = 0;
    [SerializeField, Tooltip("If ture, this Cell we be treated as empty space and Cell will be able to be built in this Cell's place.")]
    private bool _canBuildOver = false;
    [SerializeField, Tooltip("If true, this Cell will be able to be destroyed and converted to air.")]
    private bool _isDestroyable = false;
    [SerializeField]
    private EnumZMoveDirection _zMoveDirections = EnumZMoveDirection.NEITHER;
    [SerializeField, Tooltip("If true, the Cell will burn")]
    private bool _flammable = false;
    [SerializeField]
    private float _temperatureOutput = 0f;
    [SerializeField]
    private bool _supportsCeiling = false;
    [SerializeField, Tooltip("If true, the fog from this cell will be lifted if an adjacent cell has it's fog lifted.")]
    private bool _includeInFogFloodLift = false;

    [Space]

    [SerializeField, Tooltip("If true, the Object Tile will be tinted.")]
    private bool _tintObjectTile = false;

    [Space]

    [SerializeField, Tooltip("The associate Prefab that will be spawned when this Cell is placed.")]
    private GameObject _behaviorPrefab = null;

    public bool isSolid => this._isSolid;
    public bool rotationalOverride => this._rotationalOverride;
    public int movementCost => this._movementCost;
    public bool isWalkable => this.movementCost >= 0;
    public bool canBuildOver => this._canBuildOver;
    public bool isDestroyable => this._isDestroyable;
    public EnumZMoveDirection zMoveDirections => this._zMoveDirections;
    public bool isFlammable => this._flammable;
    public float temperatureOutput => this._temperatureOutput;
    public bool supportsCeiling => this._supportsCeiling;
    public bool includeInFogFloodLift => this._includeInFogFloodLift;
    public bool tintObjectTile => this._tintObjectTile;
    public GameObject behaviorPrefab => this._behaviorPrefab;

    public TileRenderData getRenderData(Rotation rotation) {
        if(this.rotationalOverride) {
            RotationOverride rotOverride = this.overrideFromRotation(rotation);

            if(rotOverride.isOverrideEnabled) {
                // If the rotational override is missing data, pull from the defaults.
                return new TileRenderData(
                    rotOverride.floorOverlay == null ? this._floorOverlayTile : rotOverride.floorOverlay,
                    rotOverride.objectTile == null ? this._objectTile : rotOverride.objectTile,
                    rotOverride.overlayTile == null ? this._objectOverlayTile : rotOverride.overlayTile,
                    rotOverride.effect);
            }
        }

        return new TileRenderData(this._floorOverlayTile, this._objectTile, this._objectOverlayTile, RotationEffect.NOTHING);
    }

    public bool isRotationOverrideEnabled(Rotation rotation) {
        return this.overrideFromRotation(rotation).isOverrideEnabled;
    }

    private RotationOverride overrideFromRotation(Rotation rotation) {
        if(rotation == Rotation.UP) {
            return this.up;
        }
        else if(rotation == Rotation.RIGHT) {
            return this.right;
        }
        else if(rotation == Rotation.DOWN) {
            return this.down;
        }
        else {
            return this.left;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CellData), true)]
    public class CellDataEditor : Editor {

        private SerializedProperty up;
        private SerializedProperty right;
        private SerializedProperty down;
        private SerializedProperty left;

        private void OnEnable() {
            this.up = this.serializedObject.FindProperty("up");
            this.right = this.serializedObject.FindProperty("right");
            this.down = this.serializedObject.FindProperty("down");
            this.left = this.serializedObject.FindProperty("left");
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            this.DrawDefaultInspector();

            CellData script = (CellData)this.target;

            if(script.rotationalOverride) {
                // Show rotation properties

                EditorGUILayout.Space(16);
                EditorGUILayout.LabelField("  -----  Rotation Overrides  -----  ", EditorStyles.boldLabel);

                this.func(this.up);
                this.func(this.right);
                this.func(this.down);
                this.func(this.left);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        private void func(SerializedProperty prop) {
            SerializedProperty p1 = prop.FindPropertyRelative("_enableOverride");

            EditorGUILayout.BeginHorizontal();
            prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.name.ToUpper());
            p1.boolValue = EditorGUILayout.Toggle("Enable override", p1.boolValue);
            EditorGUILayout.EndHorizontal();

            if(prop.isExpanded && p1.boolValue) {
                EditorGUI.indentLevel = 1;

                EditorGUILayout.PropertyField(prop.FindPropertyRelative("_floorOverlayTile"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("_objectTile"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("_overlayTile"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("_effect"));

                EditorGUI.indentLevel = 0;
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            CellData cell = (CellData)this.serializedObject.targetObject;

            if(cell == null) {
                return null;
            }

            Texture2D tex = new Texture2D(width, height);

            Texture2D[] textures = new Texture2D[] {
                this.func(cell._floorOverlayTile),
                this.func(cell._objectTile),
                this.func(cell._objectOverlayTile),
            };
            Vector2Int texSize = Vector2Int.zero;
            Color[] rootCols = null;
            for(int i = 0; i < textures.Length; i++) {
                if(textures[i] == null) {
                    continue;
                }

                if(rootCols == null) {
                    rootCols = textures[i].GetPixels();
                    texSize = new Vector2Int(textures[i].width, textures[i].height);
                } else {
                    // Add texture to the base
                    Color[] cols1 = textures[i].GetPixels();

                    if(rootCols.Length != cols1.Length) {
                        Debug.LogWarning("[" + this.name + "] Can't make preview, tile sizes do not match.");
                        Debug.LogWarning(rootCols.Length + "    " + cols1.Length);
                        return tex;
                    }

                    for(int j = 0; j < cols1.Length; ++j) {
                        if(cols1[j].a != 0) {
                            rootCols[j] = cols1[j];
                        }
                    }
                }
            }

            if(rootCols != null) {
                Texture2D root = new Texture2D(texSize.x, texSize.y);
                root.SetPixels(rootCols);
                root.Apply();

                EditorUtility.CopySerialized(root, tex);
                return tex;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Returns null if there is no assigned Tile.
        /// </summary>
        private Texture2D func(TileBase tile) {
            Sprite s = TileSpriteGetter.retrieveSprite(tile);

            if(s != null) {
                return SpriteToTexture.convert(s);
            }

            return null;
        }
    }
#endif
}
