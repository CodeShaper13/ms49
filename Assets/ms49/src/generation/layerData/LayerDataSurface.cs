using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Layer", menuName = "MS49/Generation/Layer Data Surface", order = 11)]
public class LayerDataSurface : LayerData {

    [Space]

    [SerializeField]
    private TileBase grassTile = null;

    public override Color GetGroundTint(World world, int x, int y) {
        return world.IsOutside(new Position(x, y, 0)) ? Color.white : base.GetGroundTint(world, x, y);
    }

    public override TileBase GetGroundTile(World world, int x, int y) {
        if(world.IsOutside(new Position(x, y, 0))) {
            return this.grassTile;
        } else {
            return base.GetGroundTile(world, x, y);
        }
    }

    public override CellData GetFillCell(World world, int x, int y) {
        return world.IsOutside(new Position(x, y, 0)) ? null : this._fillCell;
    }
}
