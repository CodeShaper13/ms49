using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Layer", menuName = "MS49/Generation/Layer Data", order = 10
    )]
public class LayerData : ScriptableObject {

    [SerializeField]
    protected Color tintColor = Color.white;
    [SerializeField]
    protected CellData tile = null;
    [SerializeField]
    protected TileBase floorTile = null;
    [SerializeField]
    private bool _hasFog = false;
    [SerializeField]
    private float _defaultTemperature = 0;
    [SerializeField]
    private bool _generateBlockerRocks = true;

    [Space]

    public bool generateCaves = false;
    [Range(0, 100)]
    public float caveFillPercent;
    [Min(0)]
    public int caveSmoothPases = 5;
    [SerializeField]
    private int _f1 = 4;
    [SerializeField]
    private int _f2 = 4;

    [Space]

    public OreSettings[] oreSpawnSettings;

    [Space]

    public EnumLakeType lakeType = EnumLakeType.NONE;
    [Range(0, 100)]
    [Tooltip("The percent of caves that become lakes.  Lakes only spawn on layers with generateCaves set to true.")]
    public int lakeChance = 10;

    public bool hasFog => this._hasFog;
    public float defaultTemperature => this._defaultTemperature;
    public bool generateBlockerRocks => this._generateBlockerRocks;
    public int f1 => this._f1;
    public int f2 => this._f2;

    /// <summary>
    /// Returns the Color to tint floor and wall Cells.
    /// </summary>
    public virtual Color getGroundTint(World world, int x, int y) {
        return this.tintColor;
    }

    /// <summary>
    /// Returns the Cell to fill the Layer with.
    /// </summary>
    public virtual CellData getFillCell(World world, int x, int y) {
        return this.tile;
    }

    /// <summary>
    /// Returns the Layer's ground Tile that is placed under cells without a ground tile set (Like a table).
    /// </summary>
    public virtual TileBase getGroundTile(World world, int x, int y) {
        return this.floorTile;
    }
}
