using UnityEngine;

[CreateAssetMenu(fileName = "OreVein", menuName = "MS49/Generation/Ore Vein", order = 20)]
public class OreVeinData : ScriptableObject {

    [SerializeField]
    private CellData _cell = null;
    [MinMaxSlider(1, 12)]
    public Vector2Int _veinSize = new Vector2Int(1, 1);

    public CellData cell => this._cell;
    public Vector2Int size => this._veinSize;
}
