using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskMineRock : TaskBase<EntityWorker> {

    [SerializeField, Min(0.1f)]
    private float _mineSpeed = 1f;
    [SerializeField, Min(0)]
    private float _hungerCost = 5f;
    [SerializeField, Min(0)]
    private float _energyCost = 2f;
    [SerializeField]
    private GameObject stoneCrackParticlePrefab = null;
    [SerializeField]
    private GameObject mineParticlePrefab = null;
    [SerializeField]
    private MinerMetaData minerData = null;

    private float timeMining;
    private Position stonePos;
    private Particle crackParticle;

    public override bool shouldExecute() {
        if(this.minerData.heldItem == null) {
            // No stone, Worker can mine.
            bool flag = this.findClosestStone(this.owner.getCellPos());
            if(flag) {
                return true;
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.minerData.heldItem != null) {
            return false; // Worker carrying stone.
        }
        if(!(this.owner.world.getCellState(this.stonePos).data is CellDataMineable)) {
            return false; // Targets cell is no longer a mineable cell (it was mined?).
        }
        if(!this.owner.world.targetedSquares.isTargeted(this.stonePos)) {
            return false; // No more targeted squares (Player canceled it?).
        }

        return true;
    }

    public override void preform() {
        if(!this.moveHelper.hasPath()) {

            if(this.crackParticle == null) {
                this.crackParticle = this.owner.world.particles.spawn(this.stonePos.center, this.owner.depth, this.stoneCrackParticlePrefab);
            }

            this.timeMining += Time.deltaTime;

            if(this.timeMining >= _mineSpeed) {
                // Pickup the dropped item from the stone.
                CellData data = this.owner.world.getCellState(this.stonePos).data;
                if(data is CellDataMineable) {
                    this.minerData.heldItem = ((CellDataMineable)data).droppedItem;
                }

                // Play particle (and color it)
                Particle particle = this.owner.world.particles.spawn(this.stonePos.center, this.owner.depth, this.mineParticlePrefab);
                if(particle != null) {
                    LayerData layerData = this.owner.world.mapGenerator.getLayerFromDepth(this.owner.depth);
                    ParticleSystem.MainModule main = particle.ps.main;
                    main.startColor = layerData.getGroundTint(this.owner.world, this.stonePos.x, this.stonePos.y);
                }

                // Reduce hunger and energy
                this.owner.hunger.decrease(this._hungerCost);
                this.owner.energy.decrease(this._energyCost);

                // Remove the stone.
                this.owner.world.setCell(this.stonePos, null);
                this.owner.world.targetedSquares.stopTargeting(this.stonePos);
                this.owner.world.liftFog(this.stonePos);
                this.owner.world.tryCollapse(this.stonePos);

                // Add to statistic
                this.owner.world.stoneExcavated++;
            }
        }
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.timeMining = 0;
        if(this.crackParticle != null) {
            this.owner.world.particles.remove(this.crackParticle);
            this.crackParticle = null;
        }

        //this.moveHelper.stop();
    }

    /// <summary>
    /// Finds the closest stone and sets the Miner on a path to it.  If
    /// no stone is found, false is returned.
    /// </summary>
    private bool findClosestStone(Vector2 searchOrgin) {
        HashSet<TargetedSquare> targetedPosList = this.owner.world.targetedSquares.list;

        bool foundSquare = this.method((ts) => { return ts.isPriority; });

        if(foundSquare) {
            return true;
        } else {
            return this.method((ts) => { return !ts.isPriority; });
        }
    }

    private bool method(Func<TargetedSquare, bool> func) {
        HashSet<TargetedSquare> targetedPosList = this.owner.world.targetedSquares.list;

        foreach(TargetedSquare ts in targetedPosList.OrderBy(x => x.pos.distance(this.owner.position)).ToList()) {
            if(!func(ts)) {
                continue;
            }

            // Quick check to make sure the targeted stone is not totally surrounded, as it often is
            bool fullySurrounded = true;
            foreach(Rotation r in Rotation.ALL) {
                Position neighborPos = ts.pos + r;

                if(!this.owner.world.isOutOfBounds(neighborPos) && this.owner.world.getCellState(neighborPos).data.isWalkable) {
                    fullySurrounded = false;
                    break;
                }
            }
            if(fullySurrounded) {
                continue;
            }

            Position? dest = this.moveHelper.setDestination(ts.pos, true);
            if(dest != null) {
                this.stonePos = ts.pos;
                return true; // Found square with a path.
            }
        }

        return false;
    }
}
