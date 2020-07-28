using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskMineRock : TaskBase<EntityMiner> {

    private float timeMining;
    private Position destination; // Different from stonePos, this is where the Miner is going, an adjacent tile.
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

    public override void startExecute() {
        base.startExecute();
    }

    public override bool continueExecuting() {
        bool stopExecution = false;

        if(this.owner.heldItem != null) {
            stopExecution = true; // Worker carrying stone.
        }
        if(!(this.owner.world.getCellState(this.stonePos).data is CellDataMineable)) {
            stopExecution = true; // Targets cell is no longer a mineable cell (it was mined?).
        }
        if(!this.owner.world.isTargeted(this.stonePos)) {
            stopExecution = true; // No more targeted squares (player canceled it?).
        }

        if(stopExecution) {
            this.moveHelper.stop();
            return false;
        } else {
            return true;
        }
    }

    public override void preform() {
        // Traveling to stone/mine.

        if(this.owner.position == this.destination) {
            this.timeMining += Time.deltaTime;


            if(this.timeMining >= 1.0f) {
                // Give money if they mined an ore.
                CellData data = this.owner.world.getCellState(this.stonePos.x, this.stonePos.y, this.owner.depth).data;
                if(data is CellDataMineable) {
                    this.owner.heldItem = ((CellDataMineable)data).droppedItem;
                }

                // Remove the stone.
                Position pos = new Position(this.stonePos.x, this.stonePos.y, this.owner.depth);
                this.owner.world.setCell(pos, Main.instance.tileRegistry.getAir());
                this.owner.world.setTargeted(pos, false);
                this.owner.world.liftFog(this.stonePos);
                
                this.timeMining = 0f;
            }
        }
    }

    public override void resetTask() {
        base.resetTask();

        this.timeMining = 0;
    }

    /// <summary>
    /// finds the closest stone and sets the Worker on a path to it.  If
    /// no stone is found, false is returned.
    /// </summary>
    private bool findClosestStone(Vector2 searchOrgin) {
        HashSet<Position> targetedPosList = this.owner.world.storage.targetedForRemovalSquares;

        foreach(Position targetedPos in targetedPosList.OrderBy(x => x.distance(this.owner.position)).ToList()) {
            // Quick check to make sure it's not totally surrounded, as they often are
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

            Position? destination = this.moveHelper.setDestination(targetedPos, true);
            if(destination != null) {
                this.destination = (Position)destination;
                this.stonePos = targetedPos;
                return true; // Found square with a path.
            }
        }

        return false;
    }
}
