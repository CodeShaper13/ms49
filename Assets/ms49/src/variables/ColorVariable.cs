using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Color", menuName = "Variable/Color", order = 1)]
public class ColorVariable : GenericVariable<Color> {

    public static implicit operator Color(ColorVariable d) => d == null ? new Color(0.9725491f, 0, 0.9450981f) : d.value;

#if UNITY_EDITOR
    [CustomEditor(typeof(ColorVariable), true)]
    public class ColorVariableEditor : Editor {

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            ColorVariable c = (ColorVariable)this.serializedObject.targetObject;

            Texture2D t = new Texture2D(width, height);
            Color[] colors = new Color[width * height];
            for(int i = 0; i < colors.Length; i++) {
                colors[i] = c.value;
            }
            t.SetPixels(colors);
            return t;
        }
    }
#endif
}