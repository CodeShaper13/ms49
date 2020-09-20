using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        this.onStart();
    }

    protected virtual void onStart() { }

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
    /// </summary>
    public virtual void resetTask() { }

    public virtual bool allowLowerPriority() {
        return false;
    }

    /// <summary>
    /// Attempts to set the Worker on a path to a CellBehavior of the
    /// specified type that matches the passed predicate (if not null).
    /// 
    /// Matching CellBehavior is found with a valid path, the
    /// Worker's new destination is returned and the behavior ref
    /// is set to the found behavior.
    /// </summary>
    protected Position? gotoClosestBehavior<T1>(ref T1 behavior, bool stopAdjacent, Predicate<T1> predicate = null) where T1 : CellBehavior {
        // Sort all behaviors by distance.
        List<T1> behaviors = this.owner.world.getAllBehaviors<T1>(predicate);
        behaviors = behaviors.OrderBy(
            x => x.pos.distance(this.owner.position)).ToList();

        foreach(T1 b in behaviors) {
            Position? dest = this.moveHelper.setDestination(b.pos, stopAdjacent);
            if(dest != null) {
                behavior = b;
                return dest;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns an adjacent spot that is walkable.  If none can be
    /// found, null is returned.
    /// </summary>
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
