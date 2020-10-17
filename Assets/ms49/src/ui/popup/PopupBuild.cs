using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class PopupBuild : PopupWorldReference {

    [HideInInspector]
    public List<BuildableBase> _unlockedByDefault = new List<BuildableBase>();

    [Space]

    [SerializeField]
    private GameObject _tabBtnPrefab = null;
    [SerializeField]
    private GameObject _entryBtnPrefab = null;
    [SerializeField]
    private RectTransform _tabBtnArea = null;
    [SerializeField]
    private RectTransform _buildableBtnArea = null;
    [SerializeField]
    private BuildAreaHighlighter areaHighlighter = null;
    [SerializeField]
    public SelectedCellPreview preview = null;
    [SerializeField]
    private Scrollbar scrollbar = null;
    [SerializeField]
    private Tab _miscellaneousTab = null;
    [SerializeField]
    private Text _builderRequiredMsg = null;
    [SerializeField]
    private WorkerType _workerTypeBuilder = null;

    private TabContents selectedTab;
    private BuildableBase selectedBuildable;
    private List<TabContents> tabs;
    private List<BuildableListEntry> buildableListButton;

    public Rotation rot { get; private set; }

    protected override void initialize() {
        base.initialize();

        this.tabs = new List<TabContents>();
        this.createTab(this._miscellaneousTab);

        this.buildableListButton = new List<BuildableListEntry>();

        // Add the Buildables that are unlocked by default.
        foreach(BuildableBase buildable in this._unlockedByDefault) {
            if(buildable != null) {
                this.createBuildableListButton(buildable, null);
            }
        }

        // Add the Buildables that Milestones unlock.
        foreach(MilestoneData milestone in this.world.milestones.milestones) {
            if(milestone != null) {
                foreach(BuildableBase buildable in milestone.unlockedBuildables) {
                    if(buildable != null) {
                        this.createBuildableListButton(buildable, milestone);
                    }
                }
            }
        }
    }

    protected override void onOpen() {
        base.onOpen();

        if(CameraController.instance.inCreativeMode || this.isBuilderHired()) {
            this._buildableBtnArea.gameObject.SetActive(true);
            this._tabBtnArea.gameObject.SetActive(true);
            this._builderRequiredMsg.gameObject.SetActive(false);

            // Hide the tab buttons if they would be empty
            foreach(TabContents tc in this.tabs) {
                bool atLeastOneUnlocked = false;

                if(CameraController.instance.inCreativeMode) {
                    atLeastOneUnlocked = true;
                } else if(!tc.tab.onlyInCreative) {
                    foreach(BuildableListEntry btn in tc.btns) {
                        if(btn.milestone.isUnlocked) {
                            atLeastOneUnlocked = true;
                            break;
                        }
                    }
                }

                tc.tabIconButton.gameObject.SetActive(atLeastOneUnlocked);
            }

            this.setSelectedBuildable(null);

            this.scrollbar.value = 0f;

            // Show only the Buildables that are on the open tab.
            foreach(TabContents tc in this.tabs) {
                tc.setButtonsVisible(false);
            }

            // If the selected tab is no longer visible, set the miscellaneous tab to be selected
            if(this.selectedTab == null || !this.selectedTab.tabIconButton.gameObject.activeSelf) {
                this.selectedTab = this.tabs[0]; // Miscellaneous tab.
            }

            this.setSelectedTab(this.selectedTab);
        } else {
            this._buildableBtnArea.gameObject.SetActive(false);
            this._tabBtnArea.gameObject.SetActive(false);
            this._builderRequiredMsg.gameObject.SetActive(true);
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
        
        this.areaHighlighter.hide();
    }

    public void setSelectedTab(TabContents tab) {
        if(this.selectedTab != null) {
            this.selectedTab.setButtonsVisible(false);
        }

        this.selectedTab = tab;

        this.selectedTab.setButtonsVisible(true);
    }

    /// <summary>
    /// Sets the currently selected Buildable.  This will show the 
    /// review the the build area highlight.
    /// </summary>
    public void setSelectedBuildable(BuildableBase buildable) {
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

    private TabContents createTab(Tab tab) {
        BuildableTabButton btn = GameObject.Instantiate(
            this._tabBtnPrefab,
            this._tabBtnArea).GetComponent<BuildableTabButton>();

        TabContents tc = new TabContents(tab, btn);

        btn.setTab(tc);

        this.tabs.Add(tc);

        return tc;
    }

    private void createBuildableListButton(BuildableBase buildable, MilestoneData unlockingMilestone) {
        BuildableListEntry btn = GameObject.Instantiate(
            this._entryBtnPrefab,
            this._buildableBtnArea).GetComponent<BuildableListEntry>();
        btn.setStructureData(buildable, unlockingMilestone);

        // Add the Button to the list
        if(buildable.tab == null || buildable.tab == this._miscellaneousTab) {
            this.tabs[0].addButton(btn); // 0 is the miscellaneous tab
        }
        else {
            bool added = false;
            foreach(TabContents tc in this.tabs) {
                if(buildable.tab == tc.tab) {
                    tc.addButton(btn);
                    added = true;
                    break;
                }
            }

            if(!added) {
                // Create a new tab.
                TabContents newTab = this.createTab(buildable.tab);
                newTab.addButton(btn);
            }
        }

        this.buildableListButton.Add(btn);
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

    private bool isBuilderHired() {
        foreach(EntityBase e in this.world.entities.list) {
            if(e is EntityWorker && ((EntityWorker)e).type == this._workerTypeBuilder) {
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PopupBuild))]
    public class PopupBuildEditor : Editor {

        private ReorderableList list;

        public void OnEnable() {
            this.list = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty("_unlockedByDefault"), true, true, true, true);

            list.drawElementCallback = DrawListItems; // Delegate to draw the elements on the list

            list.drawHeaderCallback = (rect) => {
                EditorGUI.LabelField(rect, "Starting Buildables");
            };
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
    }
#endif
}