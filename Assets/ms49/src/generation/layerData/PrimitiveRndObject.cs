using UnityEngine;
using System;

[Serializable]
public class PrimitiveRndObject {

    [SerializeField]
    private CellData cell = null;
    [SerializeField]
    [Range(0, 1)]
    private float chance = 0.5f;
    [SerializeField]
    private bool _randomRotation = false;

    public void getRnd(ref CellData cell, ref Rotation rotation) {
        if(UnityEngine.Random.Range(0f, 1f) < this.chance) {
            cell = this.cell;

            if(this._randomRotation) {
                rotation = Rotation.ALL[UnityEngine.Random.Range(0, 4)];
            }
        }
    }
}
