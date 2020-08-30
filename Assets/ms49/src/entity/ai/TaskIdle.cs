using System.Collections;
using UnityEngine;

/// <summary>
/// Executes when the Worker is doing nothing.
/// 
/// If they continue to do nothing for a while, they will look around
/// </summary>
public class TaskIdle : TaskBase<EntityWorker> {

    private float timeExecuting;

    //public TaskIdle(EntityWorker owner, MoveHelper moveHelper) : base(owner, moveHelper) { }

    public override bool continueExecuting() {
        throw new System.NotImplementedException();
    }

    public override void preform() {
        this.timeExecuting += Time.deltaTime;
    }

    public override bool shouldExecute() {
        return true;
    }
}
