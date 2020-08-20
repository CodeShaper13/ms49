public class Milestone {

    public MilestoneData data { get; private set; }
    public bool isLocked;
    
    public Milestone(MilestoneData data) {
        this.data = data;
        this.isLocked = true;
    }

    /// <summary>
    /// Returns true if all of the requirements for this milestone have
    /// been met.
    /// </summary>
    public bool allRequiermentMet(World world) {
        if(this.data.requirements == null) {
            return false;
        }

        foreach(MilestoneRequirerment r in this.data.requirements) {
            if(!r.isMet(world)) {
                return false;
            }
        }

        return true;
    }
}
