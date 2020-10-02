using UnityEngine;
using fNbt;
using UnityEngine.Tilemaps;

public class PlotManager : MonoBehaviour, ISaveableState {

    [SerializeField, Min(1)]
    private int _plotDiameter = 32;
    //[SerializeField]
    //private int[] _enumToPlotCountMapping = new int[] { 1, 3, 5 };
    [SerializeField]
    private int[] _plotPrices = new int[] { 10000 };
    [SerializeField]
    private Tilemap _tilemap = null;
    [SerializeField]
    private TileBase _tile = null;

    public Plot[] plots { get; private set; }
    public int mapSize { get; private set; }
    public int plotDiameter => this._plotDiameter;
    public string tagName => "plots";

    private int plotCount = 3;

    private void Awake() {
        // Scale the tilemap to be the same size as a Plot.
        this._tilemap.transform.localScale = Vector3.one * this._plotDiameter;

        this.mapSize = this.plotDiameter * this.plotCount;

        // Create all of the Plots.
        this.plots = new Plot[this.plotCount * this.plotCount];
        for(int x = 0; x < this.plotCount; x++) {
            for(int y = 0; y < this.plotCount; y++) {
                Plot plot = new Plot(
                    this.plotDiameter,
                    new Vector2Int(x, y));

                this.plots[this.plotCount * x + y] = plot;
            }
        }
    }

    private void LateUpdate() {
        // Hide outlines of owned Plots.
        foreach(Plot p in this.plots) {
            this._tilemap.SetTile(
                new Vector3Int(p.plotCoordPos.x, p.plotCoordPos.y, 0),
                p.isOwned ? null : this._tile);
        }
    }

    public void initializeFirstTime() {
        // Unlock the starting plot
        this.getPlotFromPlotCoords((int)(this.plotCount / 2), 0).isOwned = true;

        foreach(Plot plot in this.plots) {
            if(this._plotPrices == null || this._plotPrices.Length == 0) {
                plot.cost = 0;
            } else {
                plot.cost = this._plotPrices[Random.Range(0, this._plotPrices.Length - 1)];
            }
        } 
    }

    public Plot getPlotFromPlotCoords(int x, int y) {
        if(x < 0 || x >= this.plotCount || y < 0 || y >= this.plotCount) {
            return null; // Out of bounds
        }

        return this.plots[this.plotCount * x + y];
    }

    /// <summary>
    /// Returns true if the cell at the passed position is owned.
    /// </summary>
    public bool isOwned(Position pos) {
        return this.isOwned(pos.x, pos.y);
    }

    /// <summary>
    /// Returns true if the cell at the passed position is owned.
    /// </summary>
    public bool isOwned(int x, int y) {
        Plot p = this.getPlot(x, y);
        if(p != null) {
            return p.isOwned;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Returns the Plot at the passed position, or null if it is out
    /// of bounds.
    public Plot getPlot(int x, int y) {
        foreach(Plot p in this.plots) {
            if(p != null) {
                if(p.contains(x, y)) {
                    return p;
                }
            }
        }

        return null;
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("mapSize", this.mapSize);

        // Write Plot costs:
        int[] costArray = new int[this.plots.Length];
        for(int i = 0; i < this.plots.Length; i++) {
            costArray[i] = this.plots[i].cost;
        }
        tag.setTag("costs", costArray);

        // Write Plot isOwned state:
        byte[] isOwnedArray = new byte[this.plots.Length];
        for(int i = 0; i < this.plots.Length; i++) {
            isOwnedArray[i] = this.plots[i].isOwned ? (byte)1 : (byte)0;
        }
        tag.setTag("isOwned", isOwnedArray);
    }

    public void readFromNbt(NbtCompound tag) {
        this.mapSize = tag.getInt("mapSize");

        // Read Plot costs:
        int[] costArray = tag.getIntArray("costs");
        for(int i = 0; i < Mathf.Min(this.plots.Length, costArray.Length); i++) {
            this.plots[i].cost = costArray[i];
        }

        // Read Plot isOwned state:
        byte[] isOwnedArray = tag.getByteArray("isOwned");
        for(int i = 0; i < Mathf.Min(this.plots.Length, isOwnedArray.Length); i++) {
            this.plots[i].isOwned = isOwnedArray[i] == 1;
        }
    }
}
