using System;
using UnityEngine;

[Serializable]
public class OreSettings {

    [SerializeField]
    private int _veinsPerChunk = 1;
    [SerializeField]
    private OreVeinData _veinData = null;

    public CellData cell => this._veinData == null
        ? null 
        : this._veinData.cell;

    public Vector2Int size => this._veinData == null
        ? Vector2Int.one
        : this._veinData.size;

    public int veinsPerChunk => this._veinsPerChunk;
}
