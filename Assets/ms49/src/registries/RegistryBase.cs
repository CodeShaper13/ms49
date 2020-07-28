using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public abstract class RegistryBase<T> : MonoBehaviour {

    [Min(0)]
    [HideInInspector]
    [SerializeField]
    private int registrySize = 16;
    [SerializeField]
    [HideInInspector]
    private T[] elements = null;

    /// <summary>
    /// Returns the element at the passed location.
    /// If the location is out of range, null is returned.
    /// </summary>
    public virtual T getElement(int location) {
        if(location < 0 || location >= this.elements.Length) {
            return default;
        }
        return this.elements[location];
    }

    /// <summary>
    /// Returns the id of the passed element in the registry.
    /// If the passed element is not in the registry, -1 is returned.
    /// </summary>
    public virtual int getIdOfElement(T other) {
        for(int i = 0; i < this.elements.Length; i++) {
            if(EqualityComparer<T>.Default.Equals(this.elements[i], other)) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns the number of spaces in the registry.
    /// </summary>
    public int getRegistrySize() {
        return this.registrySize;
    }

#if UNITY_EDITOR
    public abstract class RegistryBaseEditor : Editor {

        private SerializedProperty objs;
        private SerializedProperty regSize;
        private bool foldoutOpen;

        protected void OnEnable() {
            this.objs = this.serializedObject.FindProperty("elements");
            this.regSize = this.serializedObject.FindProperty("registrySize");
        }

        public override void OnInspectorGUI() {
            this.DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Registry Size");
            int size = EditorGUILayout.IntField(this.regSize.intValue);
            if(size != this.regSize.intValue) {
                // Changed
                this.regSize.intValue = Mathf.Clamp(size, 0, 128);
            }
            if(GUILayout.Button("Resize Registry")) {
                this.objs.arraySize = this.regSize.intValue;
            }

            EditorGUILayout.EndHorizontal();

            this.foldoutOpen = EditorGUILayout.Foldout(this.foldoutOpen, "REGISTRY");

            if(this.foldoutOpen) {
                EditorGUI.indentLevel += 1;

                if(this.objs.arraySize == 0) {
                    EditorGUILayout.LabelField("Registry has a size of 0!");
                } else {
                    for(int i = 0; i < this.objs.arraySize; i++) {
                        SerializedProperty entry = this.objs.GetArrayElementAtIndex(i);

                        Rect r = EditorGUILayout.BeginHorizontal();

                        EditorGUIUtility.labelWidth = 20;
                        EditorGUILayout.LabelField("ID: " + i, GUILayout.Width(60));

                        EditorGUIUtility.labelWidth = 35;
                        EditorGUILayout.ObjectField(entry);

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUI.indentLevel -= 1;
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}