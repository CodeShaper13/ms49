using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Item", menuName = "MS49/Mined Item", order = 1)]
public class Item : ScriptableObject {

    public Sprite sprite;
    public string itemName = "nul";
    [Min(0), Tooltip("How much the money is worth.")]
    public int moneyValue;

#if UNITY_EDITOR
    [CustomEditor(typeof(Item), true)]
    public class ItemEditor : Editor {

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
