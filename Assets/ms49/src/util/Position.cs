using System;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[Serializable]
public struct Position {

    public const float LAYER_DISTANCE_COST = 25;

    public static Position up => new Position(0, 1, 0);
    public static Position down => new Position(0, -1, 0);
    public static Position right => new Position(1, 0, 0);
    public static Position left => new Position(-1, 0, 0);

    public int x;
    public int y;
    public int depth;

    /// <summary>
    /// The center of the Cell this Position is representing.
    /// </summary>
    public Vector2 Center => new Vector2(this.x + 0.5f, this.y + 0.5f);
    public Vector3Int AsVec3Int => new Vector3Int(this.x, this.y, this.depth);

    public Vector3 AsVec3 => new Vector3(this.x, this.y, this.depth);

    public Vector2Int AsVec2Int => new Vector2Int(this.x, this.y);

    public Vector2 AsVec2 => new Vector2(this.x, this.y);

    public Position(Vector3Int vec3) : this(vec3.x, vec3.y, vec3.z) { }

    public Position(int x, int y, int depth) {
        this.x = x;
        this.y = y;
        this.depth = depth;
    }

    public Position(Vector2Int pos, int depth) : this(pos.x, pos.y, depth) { }

    public Position(EntityBase entity) : this(entity.GetCellPos(), entity.depth) { }

    public override string ToString() {
        return "[(" + this.x + ", " + this.y + ") depth=" + this.depth + "]";
    }

    public override bool Equals(object obj) {
        return this.GetHashCode() == obj.GetHashCode();
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 47;
            hash = hash * 227 + x;
            hash = hash * 227 + y;
            hash = hash * 227 + depth;
            return hash;
        }
    }

    public Position SetX(int x) {
        return new Position(x, this.y, this.depth);
    }

    public Position SetY(int y) {
        return new Position(this.x, y, this.depth);
    }

    public Position SetDepth(int depth) {
        return new Position(this.x, this.y, depth);
    }

    /// <summary>
    /// Adds the passed values to the position.
    /// </summary>
    public Position Add(int x, int y, int depth = 0) {
        return new Position(this.x + x, this.y + y, this.depth + depth);
    }

    /// <summary>
    /// subtracts the passed value from the position.
    /// </summary>
    public Position Subtract(int x, int y, int depth = 0) {
        return new Position(this.x + x, this.y + y, this.depth + depth);
    }

    public float Distance(Position other) {
        float xyDis = this.DistanceXY(other);
        float zDis = Mathf.Abs(this.depth - other.depth);
        return xyDis + (zDis * LAYER_DISTANCE_COST);
    }

    public float DistanceXY(Position other) {
        return Vector2.Distance(this.AsVec2, other.AsVec2);
    }

    public static Position operator +(Position b, Position b1) {
        return new Position(b.x + b1.x, b.y + b1.y, b.depth);
    }

    public static Position operator -(Position b, Position b1) {
        return new Position(b.x - b1.x, b.y - b1.y, b.depth);
    }

    public static Position operator +(Position pos, Rotation r) {
        Vector2Int v = r.vector;
        return new Position(pos.x + v.x, pos.y + v.y, pos.depth);
    }

    public static Position operator -(Position b, Rotation r) {
        Vector2Int v = r.vector;
        return new Position(b.x - v.x, b.y - v.y, b.depth);
    }

    public static Position operator +(Position b, Vector2Int vec2) {
        return new Position(b.x + vec2.x, b.y + vec2.y, b.depth);
    }

    public static Position operator -(Position b, Vector2Int vec2) {
        return new Position(b.x - vec2.x, b.y - vec2.y, b.depth);
    }

    public static Position operator *(Position b, int i) {
        return new Position(b.x * i, b.y * i, b.depth);
    }

    public static Position operator /(Position b, int i) {
        return new Position(b.x / i, b.y / i, b.depth);
    }

    public static bool operator ==(Position p1, Position p2) {
        return p1.x == p2.x && p1.y == p2.y && p1.depth == p2.depth;
    }

    public static bool operator !=(Position p1, Position p2) {
        return !(p1 == p2);
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Position))]
    public class PositionDrawer : PropertyDrawer {

        // Thanks https://forum.unity.com/threads/making-a-proper-drawer-similar-to-vector3-how.385532/#post-5980577

        private const float SubLabelSpacing = 4;
        private const float BottomSpacing = 2;

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {
            pos.height -= BottomSpacing;
            label = EditorGUI.BeginProperty(pos, label, prop);
            var contentRect = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);

            GUIContent[] labels = new[] {
                new GUIContent("X"),
                new GUIContent("Y"),
                new GUIContent("Depth")
            };

            SerializedProperty[] properties = new[] {
                prop.FindPropertyRelative("x"),
                prop.FindPropertyRelative("y"),
                prop.FindPropertyRelative("depth")
            };

            DrawMultiplePropertyFields(contentRect, labels, properties);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label) + BottomSpacing;
        }

        private static void DrawMultiplePropertyFields(Rect pos, GUIContent[] subLabels, SerializedProperty[] props) {
            // backup gui settings
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;

            // draw properties
            var propsCount = props.Length;
            var width = (pos.width - (propsCount - 1) * SubLabelSpacing) / propsCount;
            var contentPos = new Rect(pos.x, pos.y, width, pos.height);
            EditorGUI.indentLevel = 0;
            for(var i = 0; i < propsCount; i++) {
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(subLabels[i]).x;
                EditorGUI.PropertyField(contentPos, props[i], subLabels[i]);
                contentPos.x += width + SubLabelSpacing;
            }

            // restore gui settings
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indent;
        }
    }
#endif
}
