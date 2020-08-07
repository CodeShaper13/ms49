using UnityEngine;
using System;

[Serializable]
public class PrimitiveRndObject {

    [SerializeField]
    private CellData cell = null;
    [SerializeField]
    [Range(0, 1)]
    public float chance = 0.5f;

    public void getRnd(ref CellData cell) {
        if(UnityEngine.Random.Range(0f, 1f) < this.chance) {
            cell = this.cell;
        }
    }
}
