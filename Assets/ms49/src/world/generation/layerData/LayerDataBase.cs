using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class LayerDataBase : ScriptableObject {

    [SerializeField]
    protected Color tintColor = Color.white;
    [SerializeField]
    protected CellData tile = null;
    [SerializeField]
    private TileBase floorTile = null;

    public bool hasFog = false;

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

    public virtual Color getGroundTint() {
        return this.tintColor;
    }

    /// <summary>
    /// Returns the Cell to fill the Layer with.
    /// </summary>
    public virtual CellData getFillCell() {
        return this.tile;
    }

    public virtual TileBase getFloorTile() {
        return this.floorTile;
    }
}
