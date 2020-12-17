using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class PersonalityValue<T> {

    [SerializeField]
    private bool _enabled = false;
    [SerializeField]
    private T _value = default;
    [SerializeField]
    private T _defaultValue = default;

    public T value => this._enabled ? this._value : this._defaultValue;

    public PersonalityValue(T defaultValue) {
        this._defaultValue = defaultValue;
        this._value = this._defaultValue;
    }

#if UNITY_EDITOR
    public abstract class PersonalityValueDrawer : PropertyDrawer {

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {
            label = EditorGUI.BeginProperty(pos, label, prop);

            SerializedProperty enabledProp = prop.FindPropertyRelative("_enabled");
            SerializedProperty valueProp = prop.FindPropertyRelative("_value");

            // Checkbox
            enabledProp.boolValue = EditorGUI.Toggle(
                new Rect(pos.x, pos.y, pos.width / 2, pos.height),
                prop.displayName,
                enabledProp.boolValue);

            // Value
            if(enabledProp.boolValue) {
                EditorGUI.PropertyField(
                    new Rect(pos.x + pos.width / 2, pos.y, pos.width / 2, pos.height),
                    valueProp, 
                    GUIContent.none);
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}
