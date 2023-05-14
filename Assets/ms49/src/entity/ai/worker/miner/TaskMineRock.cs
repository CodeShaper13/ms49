using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskMineRock : TaskMovement<EntityWorker> {

    [SerializeField, Min(0.1f)]
    private float[] _mineSpeeds = new float[] { 1f, 1.5f, 2f, };
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

            bool isCareless = this.owner.info.personality.isCareless;

            bool foundSquare = this.method((ts) => ts.isPriority || isCareless);

            if(foundSquare) {
                return true;
            } else {
                return this.method((ts) => !ts.isPriority || isCareless);
            }
        }

        return false;
    }

    public override bool continueExecuting() {
        if(this.minerData.heldItem != null) {
            return false; // Worker carrying stone.
        }
        if(!(this.owner.world.GetCellState(this.stonePos).data is CellDataMineable)) {
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
            this.owner.world.particles.Remove(this.crackParticle);
            this.crackParticle = null;
        }
    }

    public override void onDestinationArive() {
        this.crackParticle = this.owner.world.particles.Spawn(this.stonePos.Center, this.owner.depth, this.stoneCrackParticlePrefab);
        ParticleSystem.MainModule main = this.crackParticle.ps.main;
        float hardness = this._mineSpeeds[this.owner.world.GetHardness(this.stonePos)];
        main.simulationSpeed = 1f / hardness;
    }

    public override void onAtDestination() {
        this.timeMining += (Time.deltaTime * this.owner.info.personality.workSpeedMultiplyer);

        int hardness = this.owner.world.GetHardness(this.stonePos);
        if(this.timeMining >= this._mineSpeeds[hardness]) {
            // Pickup the dropped item from the stone.
            CellData data = this.owner.world.GetCellState(this.stonePos).data;
            if(data is CellDataMineable) {
                CellDataMineable dataMineable = (CellDataMineable)data;

                this.minerData.heldItem = dataMineable.DroppedItem;

                if(dataMineable.ShowParticles) {
                    // Play particle (and color it)
                    Particle particle = this.owner.world.particles.Spawn(this.stonePos.Center, this.owner.depth, this.mineParticlePrefab);
                    if(particle != null) {
                        LayerData layerData = this.owner.world.MapGenerator.GetLayerFromDepth(this.owner.depth);
                        ParticleSystem.MainModule main = particle.ps.main;
                        main.startColor = layerData.GetGroundTint(this.owner.world, this.stonePos.x, this.stonePos.y);
                    }
                }

                // Add to mined stat.
                StatisticInt stat = this.owner.world.statManager.getCellMinedStat(data);
                if(stat != null) {
                    stat.increase(1);
                }
            }            

            // Reduce hunger and energy.
            this.owner.hunger.decrease(this._hungerCost);
            this.owner.energy.decrease(this._energyCost);

            // Remove the stone.
            this.owner.world.SetCell(this.stonePos, null);
            this.owner.world.targetedSquares.stopTargeting(this.stonePos);
            this.owner.world.LiftFog(this.stonePos);
            this.owner.world.tryCollapse(this.stonePos);
        }
    }

    private bool method(Func<TargetedSquare, bool> func) {
        HashSet<TargetedSquare> targetedPosList = this.owner.world.targetedSquares.list;

        foreach(TargetedSquare ts in targetedPosList.OrderBy(x => x.pos.Distance(this.owner.position)).ToList()) {
            if(!func(ts)) {
                continue;
            }

            // Quick check to make sure the targeted stone is not totally surrounded, as it often is
            bool fullySurrounded = true;
            foreach(Rotation r in Rotation.ALL) {
                Position neighborPos = ts.pos + r;

                if(!this.owner.world.IsOutOfBounds(neighborPos) && (this.owner.world.GetCellState(neighborPos).data.IsWalkable && !this.owner.world.IsCoveredByFog(neighborPos))) {
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
