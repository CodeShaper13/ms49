using UnityEngine;

public class TaskBuild : TaskBase<EntityWorker> {

    private float timeBuilding;
    private bool isHammering;
    private CellBehaviorBuildSite buildSite;

    public override bool continueExecuting() {
        if(this.buildSite == null) {
            return false;
        }

        return true;
    }

    public override void preform() {
        if(!this.isHammering) {
            if(!this.moveHelper.hasPath()) {
                this.isHammering = true;
                this.buildSite.startPs();
            }
        } else {
            this.timeBuilding += Time.deltaTime;
            if(this.timeBuilding >= this.buildSite.constructionTime) {
                this.buildSite.placeIntoWorld();
            }
        }
    }

    public override bool shouldExecute() {
        if(this.gotoClosestBehavior<CellBehaviorBuildSite>(
            ref this.buildSite,
            true,
            (behavior) => behavior.isPrimary && !behavior.isOccupied()) != null) {

            this.buildSite.setOccupant(this.owner); // Claim it so no one takes it.
            return true;
        }

        return false;
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.timeBuilding = 0;
        this.isHammering = false;

        if(this.buildSite != null) {
            this.buildSite.stopPs();
        }
    }
}
