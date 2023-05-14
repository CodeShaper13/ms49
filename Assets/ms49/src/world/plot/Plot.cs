using UnityEngine;

public class Plot {

    public int cost { get; set; }
    public bool isOwned { get; set; }

    public RectInt rect { get; private set; }
    public Vector2Int plotCoordPos { get; private set; }

    public Plot(int plotSize, Vector2Int plotCoordPos) {
        Vector2Int plotOrigin = plotCoordPos * plotSize;
        this.rect = new RectInt(plotOrigin, new Vector2Int(plotSize, plotSize));

        this.plotCoordPos = plotCoordPos;
    }

    /// <summary>
    /// Returns true if the plot contains the passed point.
    /// </summary>
    public bool contains(int x, int y) {
        return this.rect.Contains(new Vector2Int(x, y));
    }

    /// <summary>
    /// Returns true if the plot contains the passed point.
    /// </summary>
    public bool contains(Position pos) {
        return this.rect.Contains(pos.AsVec2Int);
    }
}
