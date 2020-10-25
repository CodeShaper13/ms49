using UnityEngine;

public class TaskBuild : TaskMovement<EntityWorker> {

    [SerializeField, Min(0)]
    private float _hungerCost = 2f;
    [SerializeField, Min(0)]
    private float _energyCost = 1f;

    private float timeBuilding;
    private CellBehaviorBuildSite buildSite;

    public override bool continueExecuting() {
        if(this.buildSite == null) {
            return false;
        }

        return true;
    }

    public override bool shouldExecute() {
        this.buildSite = this.calculateAndSetPathToClosest<CellBehaviorBuildSite>(
            true,
            behavior => behavior.isPrimary && !behavior.isOccupied());

        if(this.buildSite != null) {
            this.buildSite.setOccupant(this.owner); // Claim it so no one takes it.
            return true;
        }

        return false;
    }

    public override void onDestinationArive() {
        base.onDestinationArive();

        this.buildSite.startPs();
    }

    public override void onAtDestination() {
        base.onAtDestination();

        this.timeBuilding += Time.deltaTime;
        if(this.timeBuilding >= this.buildSite.constructionTime) {
            this.buildSite.placeIntoWorld();

            this.owner.hunger.decrease(this._hungerCost);
            this.owner.energy.decrease(this._energyCost);
        }
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.timeBuilding = 0;

        if(this.buildSite != null) {
            this.buildSite.stopPs();
        }

        this.buildSite = null;
    }
}
