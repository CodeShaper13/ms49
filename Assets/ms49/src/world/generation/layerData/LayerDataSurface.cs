using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Layer", menuName = "MS49/Layer Generation/Surface", order = 1)]
public class LayerDataSurface : LayerDataBase {

    [Space]

    [SerializeField]
    private Tile grassTile = null;

    public override Color getGroundTint(int x, int y) {
        return isOutside(x, y) ? Color.white : base.getGroundTint(x, y);
    }

    public override TileBase getGroundTile(int x, int y) {
        if(isOutside(x, y)) {
            return this.grassTile;
        } else {
            return base.getGroundTile(x, y);
        }
    }

    public override CellData getFillCell(int x, int y) {
        return isOutside(x, y) ? null : this.tile;
    }

    public static bool isOutside(int x, int y) {
        float noise = Mathf.PerlinNoise(x * 0.1f, 1);
        int endY = Mathf.RoundToInt((noise * 10) + 5); // surfaceSize;
        return y < endY;
    }
}
