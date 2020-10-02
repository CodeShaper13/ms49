using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[AddComponentMenu("UI/Localized Text", 10)]
public class TextLocalized : Text {

    [SerializeField]
    private string[] _args = new string[0];

    public object[] args {
        get => this._args;
        set {
            this._args = new string[value.Length];
            for(int i = 0; i < value.Length; i++) {
                this._args[i] = value[i].ToString();
            }
        }
    }

    public override string text {
        get => base.text;
        set {
            if(Application.isPlaying) {
                base.text = I18n.translation(value, this._args);
            } else {
                base.text = value;
            }
        }
    }

    protected override void Start() {
        base.Start();

        this.text = this.text;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TextLocalized), true)]
    public class TextLocalizedEditor : UnityEditor.UI.TextEditor {

        private SerializedProperty args;

        protected override void OnEnable() {
            base.OnEnable();

            this.args = this.serializedObject.FindProperty("_args");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            this.serializedObject.Update();

            EditorGUILayout.Space(20);

            EditorGUILayout.PropertyField(this.args);

            this.serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}
