using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class TaskMovement<T> : TaskBase<T> where T : EntityBase {

    private NavPath _navPath;
    private bool onReachCallbackCalled = false;

    public NavPath navPath {
        get {
            return this._navPath;
        }
        set {
            this._navPath = value;
            this.onReachCallbackCalled = false;
        }
    }

    public override void onTaskStart() {
        base.onTaskStart();

        if(this.navPath != null) {
            this.agent.setPath(this.navPath);
        }
    }

    public override void preform() {
        if(this.navPath != null) {
            Position endPos = this.navPath.endPoint.position;
            if(this.owner.depth == endPos.depth && Vector2.Distance(this.owner.worldPos, endPos.center) == 0) {
                // At the end of the path
                if(!this.onReachCallbackCalled) {
                    this.onReachCallbackCalled = true;
                    this.onDestinationArive();
                }

                this.onAtDestination();
            }
        }
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.agent.stop();

        this.navPath = null;
    }

    /// <summary>
    /// Called when the Entity frist reaches their destination.
    /// </summary>
    public virtual void onDestinationArive() { }

    /// <summary>
    /// Called every frame where the Entity is at their destination
    /// </summary>
    public virtual void onAtDestination() { }

    protected bool calculateAndSetPath(Position destination, bool stopAdjacent = false, Rotation endingRot = null) {
        NavPath path = this.agent.calculatePath(destination, stopAdjacent, endingRot);

        if(path != null) {
            this.navPath = path;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to calculate a path to a CellBehavior of the
    /// specified type that matches the passed predicate (if not null).
    /// 
    /// If a matching CellBehavior is found with a valid path, the
    /// path is set and the behavior is returned.
    /// 
    /// offset can be passed to offset the destination from the behavior.
    /// </summary>
    protected T1 calculateAndSetPathToClosest<T1>(bool stopAdjacent, Predicate<T1> predicate = null, Rotation endingRot = null, Position? offset = null) where T1 : CellBehavior {
        T1 behavior = null;

        NavPath path = this.calculatePathToClosest(ref behavior, stopAdjacent, predicate, endingRot, offset);

        if(path != null) {
            this.navPath = path;
            return behavior;
        }

        return null;
    }

    /// <summary>
    /// Attempts to calculate a path to a CellBehavior of the
    /// specified type that matches the passed predicate (if not null).
    /// 
    /// If a path is found to a matching CellBehavior, the ref is set
    /// and the NavPath is returned.
    /// </summary>
    protected NavPath calculatePathToClosest<T1>(ref T1 behavior, bool stopAdjacent, Predicate<T1> predicate = null, Rotation endingRot = null, Position? offset = null) where T1 : CellBehavior {
        // Sort all behaviors by distance.
        List<T1> behaviors = this.owner.world.getAllBehaviors<T1>(predicate);
        behaviors = behaviors.OrderBy(
            x => x.pos.distance(this.owner.position)).ToList();

        foreach(T1 b in behaviors) {
            Position pos = b.pos;

            if(offset != null) {
                pos += (Position)offset;
            }

            NavPath path = this.agent.calculatePath(pos, stopAdjacent, endingRot);

            if(path != null) {
                behavior = b;
                return path;
            }
        }

        return null;
    }
}
