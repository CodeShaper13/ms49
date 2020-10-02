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

    private BuildableBase selectedBuildable;
    public Rotation rot { get; private set; }

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
                    if(buildable == null) {
                        continue;
                    }

                    BtnStructureListEntry btn = GameObject.Instantiate(this.btnPrefab).GetComponent<BtnStructureListEntry>();
                    btn.transform.SetParent(this.area, false);
                    btn.setStructureData(buildable);

                    btnCount++;
                }
            }
        }

        this.scrollbar.value = 0f;
    }

    private void rotateUntil(bool counterclockwise) {
        Rotation startRot = this.rot;

        while(true) {
            this.rot = counterclockwise ? this.rot.counterClockwise() : this.rot.clockwise();

            if(this.selectedBuildable.isRotationValid(this.rot)) {
                return;
            }

            if(this.rot == startRot) {
                return;
            }
        }
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.selectedBuildable != null) {
            if(this.selectedBuildable.isRotatable()) {
                bool rPressed = Input.GetKeyDown(KeyCode.R);

                if(rPressed) {
                    this.rotateUntil(Input.GetKey(KeyCode.LeftShift));
                }
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

    public void setSelected(BuildableBase buildable) {
        this.selectedBuildable = buildable;
        if(this.selectedBuildable != null) {
            if(this.selectedBuildable.isRotatable()) {
                this.rot = Rotation.UP;
                foreach(Rotation r in Rotation.ALL) {
                    if(this.selectedBuildable.isRotationValid(r)) {
                        this.rot = r;
                        break;
                    }
                }
            } else {
                this.rot = null;
            }
        }

        if(this.selectedBuildable == null) {
            this.preview.hide();

            this.areaHighlighter.hide();
        } else {
            this.preview.show(this.selectedBuildable);
            this.callback_buildClick();
        }
    }

    public void callback_buildClick() {
        this.areaHighlighter.setBuildable(this.selectedBuildable);
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
            this.list = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty("unlockedByDefault"), true, true, true, true);

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
