using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class PopupBuild : PopupWindow {

    [Space]

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
    [SerializeField]
    private NodeGraphTechTree _graphTechTree = null;

    [Space]

    [SerializeField, NaughtyAttributes.ReorderableList]
    private List<BuildableBase> _creativeOnlyBuildables = null;

    private BuildableBase selectedBuildable;
    private List<BuildableListEntry> buildableListButton;
    private BuildTabToggle[] tabs;
    private BuildTabToggle selectedTab;

    public Rotation rot { get; private set; }

    private void Awake() {
        this.tabs = this.GetComponentsInChildren<BuildTabToggle>();
        this.buildableListButton = new List<BuildableListEntry>();
    }

    private void Start() {
        // Add the Buildables that are creative only.
        foreach(BuildableBase buildable in this._creativeOnlyBuildables) {
            if(buildable != null) {
                this.CreateBuildableEntry(buildable, null);
            }
        }

        // Add the Buildables from Technologies.
        foreach(Node node in this._graphTechTree.nodes) {
            if(node is NodeTechTree nodeTechTree) {
                foreach(BuildableBase buildable in nodeTechTree.UnlockedBuildables) {
                    if(buildable == null) {
                        continue;
                    }

                    this.CreateBuildableEntry(buildable, nodeTechTree);
                }
            }
        }

        foreach(BuildableListEntry entry in this.buildableListButton) {
            entry.gameObject.SetActive(false);
        }
    }

    protected override void onOpen() {
        base.onOpen();

        this.setSelectedBuildable(null);

        if(CameraController.instance.inCreativeMode || this.isBuilderHired()) {
            this._buildableBtnArea.gameObject.SetActive(true);
            this._tabBtnArea.gameObject.SetActive(true);
            this._builderRequiredMsg.gameObject.SetActive(false);

            // Hide the tab buttons if they would be empty
            foreach(BuildTabToggle tab in this.tabs) {
                bool atLeastOneUnlocked = false;

                if(CameraController.instance.inCreativeMode) {
                    atLeastOneUnlocked = true;
                }
                else if(!tab.TabData.onlyInCreative) {
                    foreach(BuildableListEntry btn in tab.buildableEntries) {
                        if(Main.instance.activeWorld.technologyTree.IsTechnologyUnlocked(btn.unlockingTechnology)) {
                            atLeastOneUnlocked = true;
                            break;
                        }
                    }
                }

                tab.gameObject.SetActive(atLeastOneUnlocked);
            }

            this.scrollbar.value = 0f;

            // Show only the Buildables that are on the open tab.
            foreach(BuildTabToggle tab in this.tabs) {
                tab.SetButtonsVisible(false, Main.instance.activeWorld.technologyTree);
            }

            // If the selected tab is no longer visible, set the miscellaneous tab to be selected
            if(this.selectedTab == null || !this.selectedTab.gameObject.activeSelf) {
                this.selectedTab = this.tabs[0]; // Miscellaneous tab.
            }

            //this.Callback_SetSelectedTab(this.selectedTab);
        } else {
            this._buildableBtnArea.gameObject.SetActive(false);
            this._tabBtnArea.gameObject.SetActive(false);
            this._builderRequiredMsg.gameObject.SetActive(true);
        }

        this.Callback_SetSelectedTab(this.selectedTab == null ? this.tabs[0] : this.selectedTab);
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.selectedBuildable != null) {
            if(this.selectedBuildable.IsRotatable()) {
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

    public void Callback_SetSelectedTab(BuildTabToggle tab) {
        if(this.selectedTab != null) {
            this.selectedTab.SetButtonsVisible(false, Main.instance.activeWorld.technologyTree);
        }

        this.selectedTab = tab;

        this.selectedTab.SetButtonsVisible(true, Main.instance.activeWorld.technologyTree);
    }

    /// <summary>
    /// Sets the currently selected Buildable.  This will show the 
    /// review the the build area highlight.
    /// </summary>
    public void setSelectedBuildable(BuildableBase buildable) {
        this.selectedBuildable = buildable;
        if(this.selectedBuildable != null) {
            if(this.selectedBuildable.IsRotatable()) {
                if(this.selectedBuildable.displayRotation == EnumRotation.NONE) {
                    this.rot = Rotation.RIGHT;
                } else {
                    this.rot = Rotation.fromEnum(this.selectedBuildable.displayRotation);
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

    private void CreateBuildableEntry(BuildableBase buildable, NodeTechTree unlockingTechnology) {
        BuildableListEntry btn = GameObject.Instantiate(
            this._entryBtnPrefab,
            this._buildableBtnArea).GetComponent<BuildableListEntry>();
        btn.gameObject.SetActive(true);
        btn.setStructureData(buildable, unlockingTechnology);

        // Add the Button to the list
        if(buildable.Tab == null || buildable.Tab == this._miscellaneousTab) {
            this.tabs[0].buildableEntries.Add(btn); // 0 is the miscellaneous tab
        }
        else {
            foreach(BuildTabToggle tab in this.tabs) {
                if(buildable.Tab == tab.TabData) {
                    tab.buildableEntries.Add(btn);
                    break;
                }
            }
        }

        this.buildableListButton.Add(btn);
    }

    private void rotateUntil(bool counterclockwise) {
        Rotation startRot = this.rot;

        while(true) {
            this.rot = counterclockwise ? this.rot.counterClockwise() : this.rot.clockwise();

            if(this.selectedBuildable.IsRotationValid(this.rot)) {
                return;
            }

            if(this.rot == startRot) {
                return;
            }
        }
    }

    private bool isBuilderHired() {
        foreach(EntityBase e in Main.instance.activeWorld.entities.list) {
            if(e is EntityWorker entityWorker && entityWorker.type == this._workerTypeBuilder) {
                return true;
            }
        }

        return false;
    }
}