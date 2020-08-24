using System;
using UnityEngine;

[Serializable]
public class OreSettings {

    [SerializeField]
    private CellData _cell = null;
    public int veinCount = 1;
    [MinMaxSlider(1, 12)]
    public Vector2Int veinSize = new Vector2Int(1, 1);

    public CellData cell { get { return this._cell; } }
}
