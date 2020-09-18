using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class LayerDataBase : ScriptableObject {

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

    [Space]

    public OreSettings[] oreSpawnSettings;

    [Space]

    public LakeType lakeType = LakeType.NONE;
    [Range(0, 100)]
    [Tooltip("The percent of caves that become lakes.  Lakes only spawn on layers with generateCaves set to true.")]
    public int lakeChance = 10;

    public bool hasFog => this._hasFog;
    public float defaultTemperature => this._defaultTemperature;
    public bool generateBlockerRocks => this._generateBlockerRocks;

    public virtual Color getGroundTint(int x, int y) {
        return this.tintColor;
    }

    /// <summary>
    /// Returns the Cell to fill the Layer with.
    /// </summary>
    public virtual CellData getFillCell(int x, int y) {
        return this.tile;
    }

    /// <summary>
    /// Returns the Layer's ground Tile that is placed under cells without a ground tile set (Like a table).
    /// </summary>
    public virtual TileBase getGroundTile(int x, int y) {
        return this.floorTile;
    }
}
