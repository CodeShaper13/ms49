using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "MS49/Structure/Surface", order = 1)]
public class StructureSurface : StructureBase {

    [SerializeField]
    private int _surfaceSize = 2;
    [SerializeField]
    private PrimitiveRndObject[] rndObjects = null;

    public override void generate(World world, int depth) {
        LayerData layerData = world.mapGenerator.getLayerFromDepth(depth);

        // Generate the inside/outside map
        for(int x = 0; x < world.mapSize; x++) {
            for(int y = 0; y < world.mapSize; y++) {
                Random.State state = Random.state;
                Random.InitState(world.seed);

                float noise = Mathf.PerlinNoise(x * 0.1f, Random.value * 1000);
                float endY = (noise * 7) + this._surfaceSize;

                Random.state = state;

                world.storage.setOutside(x, y, y < endY);
            }
        }

        for(int x = 0; x < world.mapSize; x++) {
            for(int y = 0; y < world.mapSize; y++) {
                Position pos = new Position(x, y, depth);

                if(world.isOutside(pos)) {
                    // Outside Tile.

                    CellData cell = null;
                    Rotation rotation = Rotation.UP;
                    if(world.isOutside(new Position(x, y + 1, depth))) {
                        foreach(PrimitiveRndObject pro in this.rndObjects) {
                            pro.getRnd(ref cell, ref rotation);
                        }

                    }

                    world.setCell(pos, cell, rotation);

                    // Remove fog from outside
                    world.liftFog(pos, false);
                } else {
                    // Inside Tile

                    // Remove ores exposed to the surface, but not the bedrock
                    if(x != 0 && x != world.mapSize -1 && world.isOutside(new Position(x, y - 1, depth))) {
                        world.setCell(pos, layerData.getFillCell(world, x, y));
                    }
                }
            }
         }
    }
}
