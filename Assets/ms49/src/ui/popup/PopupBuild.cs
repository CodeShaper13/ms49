using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PopupBuild : PopupWorldReference {

    [HideInInspector]
    public List<BuildableBase> unlockedByDefault = new List<BuildableBase>();

    [Space]

    [SerializeField]
    private GameObject btnPrefab = null;
    [SerializeField]
    private RectTransform area = null;
    [SerializeField]
    private BuildAreaHighlighter areaHighlighter = null;
    [SerializeField]
    public SelectedCellPreview preview = null;
    [SerializeField]
    private Scrollbar scrollbar = null;

    private BuildableBase sData;
    public Rotation rot { get; private set; }

    protected override void initialize() {
        base.initialize();
    }

    protected override void onOpen() {
        base.onOpen();

        this.setSelected(null);

        // Add all of the buttons
        RectTransform rt = this.area.GetComponent<RectTransform>();

        int btnCount = 0;

        // Add the Buildables that are unlocked by default.
        foreach(BuildableBase buildable in this.unlockedByDefault) {
            if(buildable != null) {
                this.addBtn(buildable, ref btnCount);
            }
        }

        // Add the Buildables Milestones unlock.
        foreach(MilestoneData milestone in this.world.milestones.milestones) {
            if(milestone == null) {
                continue;
            }

            if(CameraController.instance.inCreativeMode || milestone.isUnlocked) {
                foreach(BuildableBase buildable in milestone.unlockedBuildables) {
                    BtnStructureListEntry btn = GameObject.Instantiate(this.btnPrefab).GetComponent<BtnStructureListEntry>();
                    btn.transform.SetParent(this.area, false);
                    btn.setStructureData(buildable);

                    btnCount++;
                }
            }
        }

        /*
        foreach(BuildableBase buildable in this.buildings) {
            if(buildable == null) {
                continue;
            }

            if(CameraController.instance.inCreativeMode || (buildable.unlockedAt == null || buildable.unlockedAt.isUnlocked)) {
                BtnStructureListEntry btn = GameObject.Instantiate(this.btnPrefab).GetComponent<BtnStructureListEntry>();
                btn.transform.SetParent(this.area, false);
                btn.setStructureData(buildable);

                btnCount++;
            }
        }
        */

        this.scrollbar.value = 1f;
    }

    protected override void onUpdate() {
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

    protected override void onClose() {
        base.onClose();

        // Destroy generated ui elements.
        foreach(Transform t in this.area.transform) {
            GameObject.Destroy(t.gameObject);
        }

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

    private void addBtn(BuildableBase buildable, ref int btnCount) {
        BtnStructureListEntry btn = GameObject.Instantiate(this.btnPrefab).GetComponent<BtnStructureListEntry>();
        btn.transform.SetParent(this.area, false);
        btn.setStructureData(buildable);

        btnCount++;
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
