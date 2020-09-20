using UnityEngine;

public class TaskBuild : TaskBase<EntityWorker> {

    [SerializeField]
    private float buildSpeed = 4f;

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
            if(this.timeBuilding >= buildSpeed) {
                this.buildSite.placeIntoWorld();
            }
        }
    }

    public override bool shouldExecute() {
        foreach(CellBehaviorBuildSite site in this.owner.world.getAllBehaviors<CellBehaviorBuildSite>()) {
            if(site.isPrimary && !site.isOccupied()) {
                if(this.moveHelper.setDestination(site.pos, true) != null) {
                    this.buildSite = site;
                    this.buildSite.setOccupant(this.owner); // Claim it so no one takes it.
                    return true;
                }
            }
        }

        return false;
    }

    public override void resetTask() {
        base.resetTask();

        this.timeBuilding = 0;
        this.isHammering = false;
    }
}
