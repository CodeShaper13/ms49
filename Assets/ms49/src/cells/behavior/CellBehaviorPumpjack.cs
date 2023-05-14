using fNbt;
using System;
using UnityEngine;

public class CellBehaviorPumpjack : CellBehavior, IHasData {

    [SerializeField, Tooltip("How often a single unit of oil is produced.")]
    private float _drillSpeed = 1f;
    [SerializeField]
    private FloatVariable _drillSpeedMultiplyer = null;

    private float drillProgress;

    private void Update() {
        if(this.IsAboveOil()) {
            this.drillProgress += Time.deltaTime * (this._drillSpeedMultiplyer != null ? this._drillSpeedMultiplyer.value : 1);
        
            if(this.drillProgress >= this._drillSpeed) {
                this.drillProgress = 0;

                // TODO oil is produced!
            }
        }
    }

    public override void onNeighborChange(CellState triggererCell, Position triggererPos) {
        base.onNeighborChange(triggererCell, triggererPos);

        // TODO update the connected pipe, if any.
    }

    public void ReadFromNbt(NbtCompound tag) {
        this.drillProgress = tag.getFloat("drillProgress");
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.setTag("drillProgress", this.drillProgress);
    }

    /// <summary>
    /// Returns true if hte pump jack is above a non-depleted source of oil.
    /// </summary>
    private bool IsAboveOil() {
        return true; // TODO
    }
}