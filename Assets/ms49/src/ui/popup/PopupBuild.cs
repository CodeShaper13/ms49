using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class PopupBuild : PopupWindow {

    [SerializeField]
    [HideInInspector]
    private List<BuildableBase> buildings = new List<BuildableBase>();

    [Space]

    [SerializeField]
    private GameObject btnPrefab = null;
    [SerializeField]
    private RectTransform area = null;
    [SerializeField]
    private BuildAreaHighlighter areaHighlighter = null;
    [SerializeField]
    public SelectedCellPreview preview = null;

    private BuildableBase sData;
    public Rotation rot { get; private set; }

    public override void onAwake() {
        this.setSelected(null);

        // Add all of the button
        const float BTN_HEIGHT = 35f;

        RectTransform rt = this.area.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, this.buildings.Count * BTN_HEIGHT);

        foreach(BuildableBase c in this.buildings) {
            if(c != null) {
                BtnStructureListEntry btn = GameObject.Instantiate(this.btnPrefab).GetComponent<BtnStructureListEntry>();
                btn.transform.SetParent(this.area, false);
                btn.setStructureData(c);
            }
        }
    }

    public override void onUpdate() {
        base.onUpdate();

        if(this.sData != null && this.sData.isRotatable()) {
            bool rPressed = Input.GetKeyDown(KeyCode.R);
            if(rPressed && Input.GetKey(KeyCode.LeftShift)) {
                this.rot = this.rot.counterClockwise();
            } else if(rPressed) {
                this.rot = this.rot.clockwise();
            }
        }
    }

    public override void onClose() {
        base.onClose();

        this.areaHighlighter.hide();
    }

    public void setSelected(BuildableBase sData) {
        this.sData = sData;
        if(this.sData != null) {
            if(this.sData.isRotatable()) {
                this.rot = Rotation.UP;
            } else {
                this.rot = null;
            }
        }

        if(this.sData == null) {
            this.preview.hide();

            this.areaHighlighter.hide();
        } else {
            this.preview.show(this.sData);
            this.callback_buildClick();
        }
    }

    public void callback_buildClick() {
        this.areaHighlighter.setBuildable(this.sData);
        this.areaHighlighter.show();
        this.preview.hide();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PopupBuild))]
    public class PopupBuildEditor : Editor {

        private PopupBuild popup;
        private ReorderableList list;

        public void OnEnable() {
            this.popup = (PopupBuild)this.target;
            this.list = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty("buildings"), true, true, true, true);

            list.drawElementCallback = DrawListItems; // Delegate to draw the elements on the list
            list.drawHeaderCallback = DrawHeader; // Skip this line if you set displayHeader to 'false' in your ReorderableList constructor.
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            this.serializedObject.Update();
            this.list.DoLayoutList();
            this.serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(this.target);
        }

        // Draws the elements on the list
        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

            EditorGUI.ObjectField(new Rect(rect.x, rect.y, 300, EditorGUIUtility.singleLineHeight), element);
        }

        private void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Structures");
        }
    }
#endif
}
