using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Layer", menuName = "MS49/Generation/Layer Data", order = 10)]
public class LayerData : ScriptableObject {

    [SerializeField, Tooltip("The color that rock is tinted.")]
    protected Color tintColor = Color.white;
    [SerializeField, Required]
    protected CellData _fillCell = null;
    [SerializeField, Required]
    protected TileBase floorTile = null;
    [SerializeField]
    private bool _hasFog = false;
    [SerializeField, Min(0)]
    private float _defaultTemperature = 0;

    [Space]

    public OreSettings[] oreSpawnSettings;

    public bool HasFog => this._hasFog;
    public float DefaultTemperature => this._defaultTemperature;

    /// <summary>
    /// Returns the Color to tint floor and wall Cells.
    /// </summary>
    public virtual Color GetGroundTint(World world, int x, int y) {
        return this.tintColor;
    }

    /// <summary>
    /// Returns the Cell to fill the Layer with.
    /// </summary>
    public virtual CellData GetFillCell(World world, int x, int y) {
        return this._fillCell;
    }

    /// <summary>
    /// Returns the Layer's ground Tile that is placed under cells without a ground tile set (Like a table).
    /// </summary>
    public virtual TileBase GetGroundTile(World world, int x, int y) {
        return this.floorTile;
    }
}
