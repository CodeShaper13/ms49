using System.Collections;
using UnityEngine;

public abstract class TaskBase<T> : MonoBehaviour, ITask where T : EntityBase {

    [SerializeField]
    private int _priority = 0;

    protected T owner;
    protected MoveHelper moveHelper;

    public int priority => this._priority;

    private void Start() {
        this.owner = this.GetComponentInParent<T>();
        this.moveHelper = this.GetComponentInParent<MoveHelper>();
    }

    /// <summary>
    /// Called every frame the task is not running.
    /// If true is returned, that task will start running that frame.
    /// </summary>
    public abstract bool shouldExecute();

    /// <summary>
    /// Called when the task first starts executing.
    /// </summary>
    public virtual void startExecute() {
    }

    /// <summary>
    /// Called every frame that the task is running to continue "running" it.
    /// </summary>
    public abstract void preform();

    /// <summary>
    /// Called every frame to determine if the AI should execute.
    /// </summary>
    public abstract bool continueExecuting();

    /// <summary>
    /// Called when the task has stopped running.  This should reset the
    /// task, so it can be run again later on.
    /// 
    /// This is also called from the constructor.
    /// </summary>
    public virtual void resetTask() { }

    public virtual bool allowLowerPriority() {
        return false;
    }

    protected Position? getFreeSpot(Position pos) {
        foreach(Rotation r in Rotation.ALL) {
            Position pos1 = pos + r;
            if(this.owner.world.getCellState(pos1).data.isWalkable) {
                return pos1;
            }
        }
        return null;
    }
}
