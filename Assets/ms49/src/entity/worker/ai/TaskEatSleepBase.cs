using UnityEngine;

public abstract class TaskEatSleepBase<T> : TaskBase<EntityWorker> where T : CellBehaviorOccupiable {

    protected CellBehaviorOccupiable occupiable;
    protected bool rechargingAtSpot;

    private readonly float rechargeSpeed;
    private readonly float valueToStartHuntAt;
    private readonly float valueToStop;

    public TaskEatSleepBase(EntityWorker owner, MoveHelper moveHelper, float rechargeSpeed, float valueToStartHunt, float valueToStop) : base(owner, moveHelper) {
        this.rechargeSpeed = rechargeSpeed;
        this.valueToStartHuntAt = valueToStartHunt;
        this.valueToStop = valueToStop;
    }

    public override bool continueExecuting() {
        return this.getStat() < this.valueToStop;
    }

    public override void preform() {
        if(this.moveHelper.getDistanceToEnd() <= 0.1f) {
            if(!this.rechargingAtSpot) {
                occupiable.setOccupant(this.owner);
                this.rechargingAtSpot = true;
                this.onStartRecharge();
            }
        }

        if(this.rechargingAtSpot) {
            this.setStat(this.getStat() + (this.rechargeSpeed * Time.deltaTime));

            this.onRecharge();
        }
    }

    public override bool shouldExecute() {
        if(this.getStat() > this.valueToStartHuntAt) {
            return false; // They don't need food/sleep yet.
        } else {
            // Find a structure
            T behavior = this.owner.world.getClosestBehavior<T>(this.owner.position, b => !b.isOccupied());
            if(behavior != null) {
                if(this.moveHelper.setDestination(behavior.pos, false) != null) {
                    this.occupiable = behavior;
                    this.occupiable.setOccupant(this.owner); // Reserve it, so no one takes it.
                    return true;
                }
            }
            return false;
        }
    }

    public override void resetTask() {
        base.resetTask();

        if(this.occupiable != null) {
            this.occupiable.setOccupant(null);
        }

        this.owner.faces.setFace(WorkerFaces.EnumFace.ALIVE);
        this.occupiable = null;
        this.rechargingAtSpot = false;
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

    protected virtual void onStartRecharge() { }

    protected virtual void onRecharge() { }

    protected abstract float getStat();

    protected abstract void setStat(float value);
}
