using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Cell", menuName = "MS49/Cell/Cell", order = 1)]
public class CellData : ScriptableObject {

    [SerializeField]
    public TileBase groundTile;
    [SerializeField]
    public TileBase objectTile;
    [SerializeField]
    public TileBase overlayTile;

    public bool rotationalOverride;

    [SerializeField]
    private DirectionalTile up = new DirectionalTile();
    [SerializeField]
    private DirectionalTile right = new DirectionalTile();
    [SerializeField]
    private DirectionalTile down = new DirectionalTile();
    [SerializeField]
    private DirectionalTile left = new DirectionalTile();

    [Space]

    [Tooltip("If true, Workers will be able to walk on this Cell.")]
    public bool isWalkable = false;
    [Tooltip("If ture, this Cell we be treated as empty space and Cell will be able to be built in this Cell's place.")]
    public bool canBuildOver = false;
    [Tooltip("If true, this Cell will be able to be destroyed and converted to air.")]
    public bool isDestroyable = true;
    public EnumZMoveDirection zMoveDirections = EnumZMoveDirection.NEITHER;

    [Space]

    [Tooltip("If true, the ground tile (if set) will be tinted.  If not set, the tint will be applied reguardless of this setting.")]
    public bool tintGroundTile;
    [Tooltip("If true, the object tile (if set) will be tinted.")]
    public bool tintObjectTile;

    [Tooltip("The associate Prefab that will be spawned when this Cell is placed.")]
    public GameObject behaviorPrefab;

    public TileBase getGroundTile() {
        return this.groundTile;
    }

    public DirectionalTile getObjectTile(Rotation rotation) {
        if(this.rotationalOverride) {            
            if(rotation != null) {
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
        }

        return new DirectionalTile(this.objectTile);
    }

    public TileBase getOverlayTile() {
        return this.overlayTile;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CellData), true)]
    public class CellDataEditor : Editor {

        /*
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

        public override VisualElement CreateInspectorGUI() {
            base.CreateInspectorGUI();

            var container = new VisualElement();

            // Draw the legacy IMGUI base
            var imgui = new IMGUIContainer(OnInspectorGUI);
            // container.Add(imgui);

            // Create property fields.
            // Add fields to the container.
            container.Add(
                new Label("UP"));
            container.Add(
                     new PropertyField(serializedObject.FindProperty("up")));
            return container;

        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            this.DrawDefaultInspector();

            CellData script = (CellData)target;

            if(script.rotationalOverride) {
                // Show rotation properties

                EditorGUILayout.PropertyField(this.up, true);
            }

            this.serializedObject.ApplyModifiedProperties();
        }
        */

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            CellData cell = (CellData)this.serializedObject.targetObject;

            if(cell == null) {
                return null;
            }

            Texture2D tex = new Texture2D(width, height);

            Texture2D[] textures = new Texture2D[] {
                this.func(cell.groundTile),
                this.func(cell.objectTile),
                this.func(cell.overlayTile),
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
                return this.textureFromSprite(s);
            }

            return null;
        }

        private Texture2D textureFromSprite(Sprite sprite) {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels(
                (int)sprite.rect.x,
                (int)sprite.rect.y,
                (int)sprite.rect.width,
                (int)sprite.rect.height);

            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
    }
#endif
}
