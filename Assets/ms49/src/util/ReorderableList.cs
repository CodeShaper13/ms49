using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
    using UnityEditorInternal;
#endif

public class ReorderableListAttribute : PropertyAttribute { }

[Serializable]
public class ReorderableList<T> {
    public List<T> list_;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReorderableListAttribute))]
    public class ReorderableListDrawer : PropertyDrawer {

        private ReorderableList list_;

        public void OnEnable() { }

        public override void OnGUI(Rect rect, SerializedProperty serializedProperty, GUIContent label) {
            SerializedProperty listProperty = serializedProperty.FindPropertyRelative("list_");
            ReorderableList list = GetList(listProperty);

            float height = 0f;
            for(var i = 0; i < listProperty.arraySize; i++) {
                height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
            }
            list.elementHeight = height;
            list.DoList(rect);
        }

        public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label) {
            SerializedProperty listProperty = serializedProperty.FindPropertyRelative("list_");
            return GetList(listProperty).GetHeight();
        }

        private ReorderableList GetList(SerializedProperty serializedProperty) {
            if(list_ == null) {
                list_ = new ReorderableList(serializedProperty.serializedObject, serializedProperty);

                /*
                list_.drawHeaderCallback = (rect) => {
                    EditorGUI.LabelField(rect, serializedProperty.displayName);
                };
                */

                list_.drawElementCallback = (rect, index, isActive, isFocused) => {
                    EditorGUI.PropertyField(
                        new Rect(
                            rect.x,
                            rect.y,
                            300,
                            EditorGUIUtility.singleLineHeight),
                        list_.serializedProperty.GetArrayElementAtIndex(index));
                };
            }

            return list_;
        }
    }
#endif
