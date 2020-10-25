using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskMineRock : TaskMovement<EntityWorker> {

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

            HashSet<TargetedSquare> targetedPosList = this.owner.world.targetedSquares.list;

            bool foundSquare = this.method((ts) => { return ts.isPriority; });

            if(foundSquare) {
                return true;
            } else {
                return this.method((ts) => { return !ts.isPriority; });
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

    public override void onTaskStop() {
        base.onTaskStop();

        this.timeMining = 0;
        if(this.crackParticle != null) {
            this.owner.world.particles.remove(this.crackParticle);
            this.crackParticle = null;
        }
    }

    public override void onDestinationArive() {
        this.crackParticle = this.owner.world.particles.spawn(this.stonePos.center, this.owner.depth, this.stoneCrackParticlePrefab);
    }

    public override void onAtDestination() {
        this.timeMining += Time.deltaTime;

        if(this.timeMining >= _mineSpeed) {
            // Pickup the dropped item from the stone.
            CellData data = this.owner.world.getCellState(this.stonePos).data;
            if(data is CellDataMineable) {
                CellDataMineable dataMineable = (CellDataMineable)data;

                this.minerData.heldItem = dataMineable.droppedItem;

                if(dataMineable.showParticles) {
                    // Play particle (and color it)
                    Particle particle = this.owner.world.particles.spawn(this.stonePos.center, this.owner.depth, this.mineParticlePrefab);
                    if(particle != null) {
                        LayerData layerData = this.owner.world.mapGenerator.getLayerFromDepth(this.owner.depth);
                        ParticleSystem.MainModule main = particle.ps.main;
                        main.startColor = layerData.getGroundTint(this.owner.world, this.stonePos.x, this.stonePos.y);
                    }
                }
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

            this.navPath = this.agent.calculatePath(ts.pos, true);

            if(this.navPath != null) {
                this.stonePos = ts.pos;
                return true; // Found square with a path.
            }
        }

        return false;
    }
}
