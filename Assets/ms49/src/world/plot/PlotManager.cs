using UnityEngine;
using fNbt;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PlotManager : MonoBehaviour, ISaveableState, IFirstTimeInitializer {

    [SerializeField]
    private World _world = null;
    [SerializeField]
    private Tilemap _tilemap = null;
    [SerializeField]
    private TileBase _tile = null;

    [Space]

    [SerializeField, Min(1)]
    private int _plotSize = 32;

    [Space]

    [SerializeField, Min(0)]
    private int _minPlotPrice = 2500;
    [SerializeField, Min(0)]
    private int _maxPlotPrace = 5000;
    [SerializeField]
    private int _roundFactor = 100;

    private SubPlot[,] plotMap;

    public Plot[] plots { get; private set; }
    public int PlotSize => this._plotSize;
    public int PlotCount => this._world.MapSize / this._plotSize;
    public string saveableTagName => "plots";

    private void Awake() {
        // Scale the Tilemap to be the same size as a Plot.
        this._tilemap.transform.localScale = Vector3.one * this._plotSize;

        // Create all of the Plots.
        this.plots = new Plot[this.PlotCount * this.PlotCount];
        for(int x = 0; x < this.PlotCount; x++) {
            for(int y = 0; y < this.PlotCount; y++) {
                Plot plot = new Plot(
                    this.PlotSize,
                    new Vector2Int(x, y));

                this.plots[this.PlotCount * x + y] = plot;
            }
        }




        int count = this._world.MapSize / this._plotSize;
        /*
        SubPlot2[,] plotMap = new SubPlot2[count, count];
        for(int x = 0; x < count; x++) {
            for(int y = 0; y < count; y++) {
                plotMap[x, y] = new SubPlot2(x, y);
            }
        }

        for(int i = 0; i < 8; i++) {
            Vector2Int rndPos = new Vector2Int(
                Random.Range(0, count),
                Random.Range(0, count));

            SubPlot2 plot = plotMap[rndPos.x, rndPos.y];
            if(plot.Area == 1) {
                // Merge with a random neighbor.
                Rotation r = Rotation.Random;
                Vector2Int neightborPos = rndPos + r.vector;
                if(neightborPos.x > 0 && neightborPos.y > 0 && neightborPos.x < count && neightborPos.y < count) {
                    // Merge Plots.
                    SubPlot other = this.plotMap[neightborPos.x, neightborPos.y];

                    if(plot == other) {
                        // Can't merge with self.
                        continue;
                    }

                    plot.Merge(other);
                    this.plotMap[neightborPos.x, neightborPos.y] = plot;
                }
            }
        }
        */
    }

    public int j = 29;

    [NaughtyAttributes.Button]
    private void func() {
        int count = this._world.MapSize / this._plotSize;
        this.plotMap = new SubPlot[count, count];

        for(int x = 0; x < count; x++) {
            for(int y = 0; y < count; y++) {
                this.plotMap[x, y] = new SubPlot(x, y);
            }
        }

        // Pick plots at random, to merge with a random neighbor.
        for(int i = 0; i < j; i++) {
            Vector2Int plotPos = new Vector2Int(
                Random.Range(0, count),
                Random.Range(0, count));

            SubPlot plot = this.plotMap[plotPos.x, plotPos.y];
            if(plot.Area == 1) {
                // Merge with a random neighbor.
                Rotation r = Rotation.Random;
                Vector2Int neightborPos = plotPos + r.vector;
                if(neightborPos.x >= 0 && neightborPos.y >= 0 && neightborPos.x < count && neightborPos.y < count) {
                    // Merge Plots.
                    SubPlot other = this.plotMap[neightborPos.x, neightborPos.y];

                    if(other.Area != 1 || plot == other) {
                        // Can't merge with self.
                        continue;
                    }

                    print("Merging: " + plotPos + "  " + neightborPos);

                    plot.Merge(other);
                    this.plotMap[neightborPos.x, neightborPos.y] = plot;
                }
            }
        }
    }

    private class SubPlot2 {

        public List<Vector2Int> plots;

        public int Area => this.plots.Count;

        public SubPlot2(int x, int y) {
            this.plots = new List<Vector2Int>();
            this.plots.Add(new Vector2Int(x, y));
        }
    }

    private class SubPlot {
        
        public int width = 1;
        public int height = 1;
        public Vector2Int orgin;
        public Color c;

        public int Area => this.width * this.height;

        public SubPlot(int x, int y) {
            this.orgin = new Vector2Int(x, y);
            this.width = 1;
            this.height = 1;
            this.c = Random.ColorHSV();
        }

        public void Merge(SubPlot other) {
            this.width += this.orgin.x != other.orgin.x ? 1 : 0;
            this.height += this.orgin.y != other.orgin.y ? 1 : 0;
            this.orgin = new Vector2Int(
                Mathf.Min(this.orgin.x, other.orgin.x),
                Mathf.Min(this.orgin.y, other.orgin.y));
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

    private void OnDrawGizmos() {
        if(this.plotMap != null) {
            foreach(SubPlot plot in this.plotMap) {
                Color color = plot.c;
                color.a = 0.5f;
                Gizmos.color = color;
                /*
                Gizmos.DrawCube(
                    (new Vector3(
                        plot.orgin.x + plot.width,
                        0,
                        plot.orgin.y + plot.height)) * this._plotSize,
                    new Vector3(plot.width, 0, plot.height) * this._plotSize * 0.9f);
                */

                //for(int x = 0; x < plot.width; x++) {
                //    for(int y = 0; y < plot.height; y++) {
                        Gizmos.DrawCube(
                            (new Vector3(plot.orgin.x /*+ x*/, 0, plot.orgin.y /*+ y*/)) * this._plotSize,
                            Vector3.one * this._plotSize * 0.9f);
                //    }
                //}
                Vector3 pos = new Vector3(plot.orgin.x, 0, plot.orgin.y) * this._plotSize;
                UnityEditor.Handles.Label(pos, string.Format("w{1} h{2}", plot.orgin, plot.width, plot.height));

                Gizmos.DrawRay(
                    pos,
                    new Vector3(plot.width - 1, 0, plot.height - 1) * this._plotSize);//
            }
        }
    }

    public void InitializeFirstTime(int seed) {
        Random.InitState(seed);

        // Give Plot's their prices.
        foreach(Plot plot in this.plots) {
            int cost = Random.Range(this._minPlotPrice, this._maxPlotPrace) / this._roundFactor;
            cost *= this._roundFactor;
            plot.cost = cost;
        }

        /*
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
        */
    }

    public Plot GetPlotFromPlotCoords(int x, int y) {
        if(x < 0 || x >= this.PlotCount || y < 0 || y >= this.PlotCount) {
            return null; // Out of bounds
        }

        return this.plots[this.PlotCount * x + y];
    }

    /// <summary>
    /// Returns true if the cell at the passed position is owned.
    /// </summary>
    public bool IsOwned(Position pos) {
        return this.IsOwned(pos.x, pos.y);
    }

    /// <summary>
    /// Returns true if the cell at the passed position is owned.
    /// </summary>
    public bool IsOwned(int x, int y) {
        Plot plot = this.GetPlot(x, y);
        if(plot != null) {
            return plot.isOwned;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Returns the Plot at the passed position, or null if it is out
    /// of bounds.
    /// </summary>
    public Plot GetPlot(int x, int y) {
        foreach(Plot plot in this.plots) {
            if(plot != null) {
                if(plot.Contains(x, y)) {
                    return plot;
                }
            }
        }

        return null;
    }

    public void WriteToNbt(NbtCompound tag) {
        int[] costArray = new int[this.plots.Length];
        byte[] isOwnedArray = new byte[this.plots.Length];

        for(int i = 0; i < this.plots.Length; i++) {
            Plot plot = this.plots[i];
            costArray[i] = plot.cost;
            isOwnedArray[i] = plot.isOwned ? (byte)1 : (byte)0;
        }

        tag.setTag("costs", costArray);
        tag.setTag("isOwned", isOwnedArray);
    }

    public void ReadFromNbt(NbtCompound tag) {
        int[] costArray = tag.getIntArray("costs");
        byte[] isOwnedArray = tag.getByteArray("isOwned");

        for(int i = 0; i < this.plots.Length; i++) {
            Plot plot = this.plots[i];
            plot.cost = costArray[i];
            plot.isOwned = isOwnedArray[i] == 1;
        }
    }
}
