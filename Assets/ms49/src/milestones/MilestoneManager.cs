using fNbt;
using UnityEngine;

public class MilestoneManager : MonoBehaviour {

    [SerializeField]
    private MilestoneData[] _milestoneData = null;
    [SerializeField]
    private PopupMilestones popup = null;
    [SerializeField]
    private World world = null;

    public Milestone[] milestones { get; private set; }

    private void Awake() {
        // Construct an array of Milestone objects from the data.
        this.milestones = new Milestone[this._milestoneData.Length];
        for(int i = 0; i < this._milestoneData.Length; i++) {
            MilestoneData data = this._milestoneData[i];
            this.milestones[i] = new Milestone(data);
        }
    }

    private void Update() {
        if(!Pause.isPaused()) {
            // Check if the next milestone is unlocked

            Milestone current = this.getCurrent();
            if(current != null && current.allRequiermentMet(this.world)) {
                this.popup.open();
                this.popup.playUnlockEffect();

                current.isLocked = false;
            }
        }
    }

    /// <summary>
    /// Returns the Milestone the Player is currently working on.
    /// If they have completed all of the Milestones, null is returned.
    /// </summary>
    public Milestone getCurrent() {
        foreach(Milestone milestone in this.milestones) {
            if(milestone.isLocked) {
                return milestone;
            }
        }

        return null;
    }

    /// <summary>
    /// Checks if the passed Milestone is unlocked.  If the passed
    /// Milestone is not registered, false is returned.
    /// </summary>
    public bool isUnlocked(MilestoneData milestoneData) {
        foreach(Milestone m in this.milestones) {
            if(m.data == milestoneData) {
                return !m.isLocked;
            }
        }
        return false;
    }

    public void writeToNbt(NbtCompound tag) {
        int[] milestoneLockFlags = tag.getIntArray("milestoneLockFlags");
        for(int i = 0; i < this.milestones.Length; i++) {
            if(i >= milestoneLockFlags.Length) {
                // No data about this milestone, assume it's locked
                this.milestones[i].isLocked = true;
            } else {
                this.milestones[i].isLocked = milestoneLockFlags[i] == 1;
            }
        }
    }

    public void readFromNbt(NbtCompound tag) {
        int[] lockFlags = new int[this.milestones.Length];
        for(int i = 0; i < this.milestones.Length; i++) {
            lockFlags[i] = this.milestones[i].isLocked ? 1 : 0;
        }
        tag.setTag("milestoneLockFlags", lockFlags);
    }
}
