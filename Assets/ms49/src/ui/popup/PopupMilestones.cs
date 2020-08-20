using UnityEngine;
using UnityEngine.UI;

public class PopupMilestones : PopupWindow {

    [SerializeField]
    private RectTransform milestoneBtnArea = null;
    [SerializeField]
    private RectTransform unlockedArea = null;
    [SerializeField]
    private GameObject prefabMilestoneProgressSlider = null;
    [SerializeField]
    private GameObject prefabBuildableRenderer = null;
    [SerializeField]
    private World world = null;
    [SerializeField]
    private PopupBuild popupBuild = null;
    [SerializeField]
    private Animator animator = null;

    private Milestone currentMilestone;

    public override void onAwake() {
        base.onAwake();
    }

    public override void onOpen() {
        base.onOpen();

        this.currentMilestone = this.world.milestones.getCurrent();

        // Create progress bars for each of the requirements
        foreach(MilestoneRequirerment r in this.currentMilestone.data.requirements) {
            if(r == null) {
                continue;
            }

            MilestoneProgressSlider slider = GameObject.Instantiate(
                this.prefabMilestoneProgressSlider,
                this.milestoneBtnArea).GetComponent<MilestoneProgressSlider>();
            slider.setRequirement(r, this.world);
        }

        // Show what the Milestone unlocks
        foreach(BuildableBase buildable in this.popupBuild.buildings) {
            if(buildable.unlockedAt == null) { // Always unlocked
                continue;
            }

            if(buildable.unlockedAt == this.currentMilestone.data) {
                GameObject obj = GameObject.Instantiate(
                    this.prefabBuildableRenderer,
                    this.unlockedArea);

                obj.GetComponent<BuildableUiRenderer>().setBuildable(buildable);
                obj.GetComponent<Tooltip>().text = buildable.getName();
            }
        }
    }

    /// <summary>
    /// Plays the Milestone Unlock effect.  This will not open the
    /// Popup, it must be opened with Popup#open().
    /// </summary>
    public void playUnlockEffect() {
        this.animator.enabled = true;
    }

    public override void onClose() {
        base.onClose();

        // Destroy generated ui elements.
        foreach(Transform t in this.milestoneBtnArea.transform) {
            GameObject.Destroy(t.gameObject);
        }
        foreach(Transform t in this.unlockedArea) {
            GameObject.Destroy(t.gameObject);
        }

        // Turn off in case animator was on
        this.animator.enabled = false;
    }
}
