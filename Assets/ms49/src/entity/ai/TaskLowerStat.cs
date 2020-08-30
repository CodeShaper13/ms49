using UnityEngine;

public class TaskLowerStat : TaskBase<EntityWorker> {

    [SerializeField]
    private float decreaseRate = 0;
    [SerializeField]
    private UnlockableStat stat = null;
    [SerializeField]
    private bool onlyExecuteWhenAwake = false;

    public override bool continueExecuting() {
        if(this.onlyExecuteWhenAwake) {
            return !this.owner.isSleeping;
        }
        return true;
    }

    public override void preform() {
        this.stat.decrease(this.decreaseRate * Time.deltaTime);
    }

    public override bool shouldExecute() {
        return this.stat != null && this.stat.isStatEnabled() && (this.onlyExecuteWhenAwake != this.owner.isSleeping);
    }

    public override bool allowLowerPriority() {
        return true;
    }
}
