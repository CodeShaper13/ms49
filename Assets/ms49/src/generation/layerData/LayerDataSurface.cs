using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Layer", menuName = "MS49/Generation/Layer Data Surface", order = 11)]
public class LayerDataSurface : LayerData {

    [Space]

    [SerializeField]
    private TileBase grassTile = null;

    public override Color getGroundTint(World world, int x, int y) {
        return world.isOutside(new Position(x, y, 0)) ? Color.white : base.getGroundTint(world, x, y);
    }

    public override TileBase getGroundTile(World world, int x, int y) {
        if(world.isOutside(new Position(x, y, 0))) {
            return this.grassTile;
        } else {
            return base.getGroundTile(world, x, y);
        }
    }

    public override CellData getFillCell(World world, int x, int y) {
        return world.isOutside(new Position(x, y, 0)) ? null : this.tile;
    }
}
