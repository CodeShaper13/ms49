using UnityEngine;

[CreateAssetMenu(
    fileName = "Requirement",
    menuName = "MS49/Milestone/Requirement/Worker Count",
    order = 1)]
public class RequirementMethodWorkerCount : RequirementMethodBase {

    [SerializeField, Tooltip("If blank, all worker types will be counted.")]
    private WorkerType workerType = null;

    public override int getProgress(World world) {
        int count = 0;

        foreach(EntityBase e in world.entities.list) {
            if(e is EntityWorker) {
                if(this.workerType == null) {
                    count++;
                } else {
                    if(((EntityWorker)e).type == this.workerType) {
                        count++;
                    }
                }
            }
        }

        return count;
    }
}
