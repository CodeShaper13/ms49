using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[Serializable]
public struct DirectionalTile {

    [Tooltip("If not set, the Cell's object tile will be used.")]
    public TileBase floorOverlay;
    public TileBase tile;
    public TileBase overlayTile;
    public RotationEffect effect;

    public DirectionalTile(TileBase floorOverlay, TileBase tile, TileBase overlayTile, RotationEffect effect) {
        this.floorOverlay = floorOverlay;
        this.tile = tile;
        this.overlayTile = overlayTile;
        this.effect = effect;
    }

    public DirectionalTile(TileBase floorOverlay, TileBase tile, TileBase overlayTile) : this(floorOverlay, tile, overlayTile, RotationEffect.NOTHING) { }

    public Matrix4x4 getMatrix() {
        return Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.Euler(0f, 0f, getRot(this.effect)),
            new Vector3(
                this.effect == RotationEffect.MIRROR_X ? -1 : 1,
                this.effect == RotationEffect.MIRROR_Y ? -1 : 1,
                1));
    }

    private float getRot(RotationEffect e) {
        switch(e) {
            case RotationEffect.ROTATE_90:
                return 90f;
            case RotationEffect.ROTATE_180:
                return 180f;
            case RotationEffect.ROTATE_270:
                return 270f;
            default:
                return 0f;
        }
    }

    /*
    [CustomPropertyDrawer(typeof(DirectionalTile))]
    public class ModularJointDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // GUI Implementation
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            // Create property container element.
            var container = new VisualElement();

            // Create property fields.
            var amountField = new PropertyField(property.FindPropertyRelative("tile"));
            var unitField = new PropertyField(property.FindPropertyRelative("effect"));
            //var nameField = new PropertyField(property.FindPropertyRelative("name"), "Fancy Name");

            // Add fields to the container.
            container.Add(amountField);
            container.Add(unitField);
            //container.Add(nameField);

            return container;
        }
    }
    */
}
