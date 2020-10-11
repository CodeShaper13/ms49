using UnityEngine;

public class WaitForPathToFinish : CustomYieldInstruction {

    private EntityWorker worker;
    private NavPath path;

    public WaitForPathToFinish(EntityWorker worker) {
        this.worker = worker;
        this.path = this.worker.moveHelper.path;
    }

    public override bool keepWaiting {
        get {
            if(this.worker.moveHelper.path == this.path) {
                if(this.path.endPoint.worldPos == this.worker.worldPos) {
                    // Worker is at the end of the path
                    return true;
                }
            }
            return false;
        }
    }
}
