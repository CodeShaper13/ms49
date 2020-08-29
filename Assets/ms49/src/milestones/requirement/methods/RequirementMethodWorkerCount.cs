using UnityEngine;

[CreateAssetMenu(
    fileName = "Requirement",
    menuName = "MS49/Milestone/Requirement/Worker Count",
    order = 1)]
public class RequirementMethodWorkerCount : RequirementMethodBase {

    public override int getProgress(World world) {
        int count = 0;

        foreach(EntityBase e in world.entities.list) {
            if(e is EntityWorker) {
                count++;
            }
        }

        return count;
    }
}
