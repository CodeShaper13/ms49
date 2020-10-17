using UnityEngine;
using fNbt;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PlotManager : MonoBehaviour, ISaveableState {

    [SerializeField, Min(1), Tooltip("The size of a Plot in Cells")]
    private int _plotDiameter = 32;
    [SerializeField, Tooltip("The number of Plots making up the map.")]
    private int _plotCount = 3;
    [SerializeField]
    private int[] _plotPrices = new int[] { 10000 };
    [SerializeField]
    private Tilemap _tilemap = null;
    [SerializeField]
    private TileBase _tile = null;

    public Plot[] plots { get; private set; }
    public int mapSize => this._plotDiameter * this._plotCount;
    public int plotCount => this._plotCount;
    public int plotDiameter => this._plotDiameter;
    public string tagName => "plots";

    private void Awake() {
        // Scale the Tilemap to be the same size as a Plot.
        this._tilemap.transform.localScale = Vector3.one * this._plotDiameter;

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

    public void initializeFirstTime(int seed) {
        Random.InitState(seed);

        // Give the plots random prices.
        List<int> list = null;
        foreach(Plot plot in this.plots) {
            if(list == null || list.Count == 0) {
                list = new List<int>(this._plotPrices);
            }

            if(this._plotPrices == null || this._plotPrices.Length == 0) {
                plot.cost = 0;
            } else {
                int index = Random.Range(0, list.Count - 1);
                plot.cost = list[index];
                list.RemoveAt(index);
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
