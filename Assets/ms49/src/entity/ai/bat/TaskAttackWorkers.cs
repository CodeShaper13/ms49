using UnityEngine;

public class TaskAttackWorkers : TaskBase<EntityBat> {

    [SerializeField]
    private float _workerSpotRange = 5f;
    [SerializeField]
    private float _attackRate = 1f;

    private EntityWorker target;
    private float attackTimer;
    private NavPath navPath;

    public override bool shouldExecute() {
        foreach(EntityBase e in this.owner.world.entities.list) {
            if(e is EntityWorker) {
                if(e.depth == this.owner.depth && Vector2.Distance(e.WorldPos, this.owner.WorldPos) < this._workerSpotRange) {
                    NavPath p = this.agent.CalculatePath(e.Position);
                    if(p != null) {
                        this.navPath = p;
                        this.agent.SetPath(p);
                        this.target = (EntityWorker)e;

                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        return this.navPath != null && this.target != null && this.target.depth == this.owner.depth;
    }

    public override void preform() {
        // Update the path if the worker moves to a new cell
        if(this.target.Position != this.navPath.EndPoint) {
            this.navPath = this.agent.CalculatePath(this.target.Position);
            this.agent.SetPath(this.navPath);

            if(this.navPath != null) {
                // The target can no longer be reached.  With the
                // navPath field being null, the task will end
                // next frame.
                return;
            }
        }

        if(this.attackTimer > 0) {
            this.attackTimer -= Time.deltaTime;
        }

        if(Vector2.Distance(this.target.WorldPos, this.owner.WorldPos) < 1f && this.attackTimer <= 0) {
            // Attack

            // TODO

            this.attackTimer = this._attackRate;
        }
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.navPath = null;
        this.target = null;
        this.attackTimer = 0;
    }
}
