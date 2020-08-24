using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskMineRock : TaskBase<EntityMiner> {

    public const float TIME_TO_MINE = 1f;

    private float timeMining;
    private Position stonePos;

    public TaskMineRock(EntityMiner owner, MoveHelper moveHelper) : base(owner, moveHelper) { }

    public override bool shouldExecute() {
        if(this.owner.heldItem == null) {
            // No stone, Worker can mine.
            bool flag = this.findClosestStone(this.owner.getCellPos());
            if(flag) {
                return true;
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.owner.heldItem != null) {
            return false; // Worker carrying stone.
        }
        if(!(this.owner.world.getCellState(this.stonePos).data is CellDataMineable)) {
            return false; // Targets cell is no longer a mineable cell (it was mined?).
        }
        if(!this.owner.world.isTargeted(this.stonePos)) {
            return false; // No more targeted squares (Player canceled it?).
        }

        return true;
    }

    public override void preform() {
        if(!this.moveHelper.hasPath()) {
            this.timeMining += Time.deltaTime;

            if(this.timeMining >= TIME_TO_MINE) {
                // Pickup the dropped item from the stone.
                CellData data = this.owner.world.getCellState(this.stonePos).data;
                if(data is CellDataMineable) {
                    this.owner.heldItem = ((CellDataMineable)data).droppedItem;
                }

                // Reduce hunger
                this.owner.reduceHunger(EntityWorker.HUNGER_COST_MINE);

                // Remove the stone.
                this.owner.world.setCell(this.stonePos, Main.instance.tileRegistry.getAir());
                this.owner.world.setTargeted(this.stonePos, false);
                this.owner.world.liftFog(this.stonePos);

                // Add to statistic
                this.owner.world.stoneExcavated++;
            }
        }
    }

    public override void resetTask() {
        base.resetTask();

        this.timeMining = 0;
    }

    /// <summary>
    /// Finds the closest stone and sets the Miner on a path to it.  If
    /// no stone is found, false is returned.
    /// </summary>
    private bool findClosestStone(Vector2 searchOrgin) {
        HashSet<Position> targetedPosList = this.owner.world.storage.targetedForRemovalSquares;

        foreach(Position targetedPos in targetedPosList.OrderBy(x => x.distance(this.owner.position)).ToList()) {
            // Quick check to make sure the targeted stone is not totally surrounded, as it often is
            bool fullySurrounded = true;
            foreach(Rotation r in Rotation.ALL) {
                Position neighborPos = targetedPos + r;
                if(this.owner.world.getCellState(targetedPos + r).data.isWalkable) {
                    fullySurrounded = false;
                    break;
                }
            }
            if(fullySurrounded) {
                continue;
            }

            Position? dest = this.moveHelper.setDestination(targetedPos, true);
            if(dest != null) {
                this.stonePos = targetedPos;
                return true; // Found square with a path.
            }
        }

        return false;
    }
}
