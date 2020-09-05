using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "MS49/Structure/Surface", order = 1)]
public class StructureSurface : StructureBase {

    [SerializeField]
    private PrimitiveRndObject[] rndObjects = null;

    public override void placeIntoWorld(World world, Position pos) {
        LayerDataBase layerData = world.mapGenData.getLayerFromDepth(pos.depth);

        for(int x = 0; x < world.mapSize; x++) {
            for(int y = 0; y < world.mapSize; y++) {
                Position pos1 = new Position(x, y, pos.depth);

                if(LayerDataSurface.isOutside(x, y)) {
                    // Outside Tile.

                    if(LayerDataSurface.isOutside(x, y + 1)) {
                        CellData cell = null;
                        foreach(PrimitiveRndObject pro in this.rndObjects) {
                            pro.getRnd(ref cell);
                        }

                        if(cell != null) {
                            world.setCell(pos1, cell);
                        }
                    }

                    // Remove fog from outside
                    world.liftFog(pos1, false);
                } else {
                    // Inside Tile

                    // Remove ores exposed to the surface
                    if(LayerDataSurface.isOutside(x, y - 1)) {
                        world.setCell(pos1, layerData.getFillCell(x, y));
                    }
                }
            }
         }
    }
}
