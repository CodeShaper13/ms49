using UnityEngine;
using UnityEngine.UI;

public class PopupMilestones : PopupWorldReference {

    [SerializeField]
    private RectTransform milestoneBtnArea = null;
    [SerializeField]
    private RectTransform unlockedArea = null;
    [SerializeField]
    private GameObject prefabMilestoneProgressSlider = null;
    [SerializeField]
    private GameObject prefabBuildableRenderer = null;
    [SerializeField]
    private Animator animator = null;
    [SerializeField]
    private ParticleSystem particles = null;
    [SerializeField]
    private AudioSource unlockAudioSource = null;

    private MilestoneData currentMilestone;

    protected override void onOpen() {
        base.onOpen();

        this.currentMilestone = this.world.milestones.getCurrent();

        if(this.currentMilestone != null) {
            // Create progress bars for each of the requirements
            foreach(MilestoneRequirerment r in this.currentMilestone.requirements) {
                if(r == null) {
                    continue;
                }

                MilestoneProgressSlider slider = GameObject.Instantiate(
                    this.prefabMilestoneProgressSlider,
                    this.milestoneBtnArea).GetComponent<MilestoneProgressSlider>();
                slider.setRequirement(r, this.world);
            }

            // Show what the Milestone unlocks
            foreach(BuildableBase buildable in this.currentMilestone.unlockedBuildables) {
                if(buildable == null) {
                    continue;
                }

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
        if(this.animator != null) {
            this.animator.enabled = true;
        }

        if(this.unlockAudioSource != null) {
            this.unlockAudioSource.Play();
        }
    }

    public void playParticles() {
        if(this.particles != null) {
            this.particles.gameObject.SetActive(true);
            this.particles.Play();
        }
    }

    protected override void onClose() {
        base.onClose();

        // Destroy generated ui elements.
        foreach(Transform t in this.milestoneBtnArea.transform) {
            GameObject.Destroy(t.gameObject);
        }
        foreach(Transform t in this.unlockedArea) {
            GameObject.Destroy(t.gameObject);
        }

        // Turn off in case animator was on
        if(this.animator != null) {
            this.animator.enabled = false;
        }
    }
}
