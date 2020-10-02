using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Item", menuName = "MS49/Item", order = 1)]
public class Item : ScriptableObject {

    public Sprite sprite;
    public string itemName = "nul";
    [Min(0), Tooltip("How much money the ore is worth.")]
    public int moneyValue;
    [SerializeField]
    private bool _includeInEconemy = false;
    [SerializeField, HideInInspector]
    private Color _graphColor = new Color(1, 0, 1, 1);

    public bool includeInEconemy => this._includeInEconemy;
    public Color graphColor => this._graphColor;

#if UNITY_EDITOR
    [CustomEditor(typeof(Item), true)]
    public class ItemEditor : Editor {

        private SerializedProperty graphColorProp;

        private void OnEnable() {
            this.graphColorProp = this.serializedObject.FindProperty("_graphColor");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            this.serializedObject.Update();

            Item item = (Item)this.target;

            if(item.includeInEconemy) {
                EditorGUILayout.PropertyField(this.graphColorProp);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            Item item = (Item)this.serializedObject.targetObject;

            if(item == null) {
                return null;
            }

            Texture2D tex = SpriteToTexture.convert(item.sprite);
            return tex;
        }
    }
#endif
}
