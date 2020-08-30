using UnityEngine;

public class TaskRegulateTemperature : TaskBase<EntityWorker> {

    [SerializeField]
    private UnlockableStat temperatureStat = null;
    [SerializeField]
    private float overheatRate = 1f;

    public override bool continueExecuting() {
        return this.temperatureStat != null;
    }

    public override void preform() {
        float cellTemp = this.owner.world.storage.getTemperature(this.owner.position);
        if(cellTemp > 0) {
            this.temperatureStat.increase(cellTemp * this.overheatRate * Time.deltaTime);
        } else {
            // Temp is 0 or lower (Cool)
            this.temperatureStat.decrease(Time.deltaTime);
        }
    }

    public override bool shouldExecute() {
        return this.temperatureStat != null;
    }

    public override bool allowLowerPriority() {
        return true;
    }
}
