﻿using fNbt;
using UnityEngine;

public class MilestoneManager : MonoBehaviour {

    [SerializeField]
    private MilestoneData[] _milestoneData = null;
    [SerializeField]
    private PopupMilestones popup = null;
    [SerializeField]
    private World world = null;

    public MilestoneData[] milestones { get { return this._milestoneData; } }

    private void Awake() {
        /*
        // Construct an array of Milestone objects from the data.
        this.milestones = new Milestone[this._milestoneData.Length];
        for(int i = 0; i < this._milestoneData.Length; i++) {
            MilestoneData data = this._milestoneData[i];
            this.milestones[i] = new Milestone(data);
        }
        */
    }

    private void Update() {
        if(!Pause.isPaused()) {
            // Check if the next milestone is unlocked

            MilestoneData current = this.getCurrent();
            if(current != null && current.allRequiermentMet(this.world)) {
                this.unlock(current, true);
            }
        }
    }

    /// <summary>
    /// Returns the Milestone the Player is currently working on.
    /// If they have completed all of the Milestones, null is returned.
    /// </summary>
    public MilestoneData getCurrent() {
        foreach(MilestoneData milestone in this._milestoneData) {
            if(!milestone.isUnlocked) {
                return milestone;
            }
        }

        return null;
    }

    /// <summary>
    /// Unlocks the passed milestone.  Safe to pass null.
    /// </summary>
    public void unlock(MilestoneData milestone, bool openPopup) {
        if(milestone == null) {
            return;
        }

        if(openPopup) {
            this.popup.open();
            this.popup.playUnlockEffect();
        }

        milestone.isUnlocked = true;
    }

    public void writeToNbt(NbtCompound tag) {
        int[] milestoneLockFlags = tag.getIntArray("milestoneUnlockFlags");
        for(int i = 0; i < this._milestoneData.Length; i++) {
            if(i >= milestoneLockFlags.Length) {
                // No data about this milestone, assume it's locked
                this._milestoneData[i].isUnlocked = false;
            } else {
                this._milestoneData[i].isUnlocked = milestoneLockFlags[i] == 1;
            }
        }
    }

    public void readFromNbt(NbtCompound tag) {
        int[] lockFlags = new int[this._milestoneData.Length];
        for(int i = 0; i < this._milestoneData.Length; i++) {
            lockFlags[i] = this._milestoneData[i].isUnlocked ? 1 : 0;
        }
        tag.setTag("milestoneUnlockFlags", lockFlags);
    }
}