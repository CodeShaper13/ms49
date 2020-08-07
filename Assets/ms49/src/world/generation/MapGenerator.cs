using UnityEngine;

public class MapGenerator {

    private World world;
    private MapGenerationData genData;
    private int seed;

    private CaveGenerator caveGenerator;
    private OreGenerator oreGenerator;

    public MapGenerator(World world, MapGenerationData genData) {
        this.world = world;
        this.genData = genData;
        this.seed = "summertimeSadness".GetHashCode();

        this.caveGenerator = new CaveGenerator(this.genData.tiles);
        this.oreGenerator = new OreGenerator(this.genData.tiles);
    }

    public void generateLayer(int depth) {
        int radius = this.world.storage.mapSize;
        int layerSeed = this.seed ^ depth;

        MapAccessor accessor = new MapAccessor(this.genData.mapSize);
        LayerDataBase layerData = this.genData.getLayerFromDepth(depth);
        System.Random rnd = new System.Random(layerSeed);

        if(layerData.generateCaves) {
            // Generate caves.  This will populate the accessor cell array with non null values.
            this.caveGenerator.generateCaves(rnd, layerData, accessor);
        } else {
            accessor.fill(layerData);
        }

        this.oreGenerator.generateOres(rnd, layerData, accessor);

        if(layerData is LayerDataUnderground) {
            LayerDataUnderground ldu = (LayerDataUnderground)layerData;

            // Generate random rocks.
            for(int i = 0; i < 10; i++) {
                int x = rnd.Next(1, radius - 1);
                int y = rnd.Next(1, radius - 1);

                /*
                // Don't let rocks spawn in the middle of the map (the start point).
                if(Vector2.Distance(new Vector2Int(x, y), this.genData.startingRoomPosition.vec2Int) <= 8) {
                    continue;
                }
                */

                // Pick the size.
                int width = rnd.Next(2, 5);
                int heigth = width + rnd.Next(-1, 2);
                if(rnd.Next(0, 2) == 0) {
                    int temp = width;
                    width = heigth;
                    heigth = temp;
                }

                // Place tiles.
                CellData cell;
                for(int x1 = 0; x1 < width; x1++) {
                    for(int y1 = 0; y1 < heigth; y1++) {
                        cell = this.genData.unbreakableRockTiles.middle;
                        accessor.setCell(x + x1, y + y1, cell);
                    }
                }
            }
        }

        Layer layer = new Layer(this.world, depth);
        this.world.storage.setLayer(layer, depth);

        int mapSize = this.world.storage.mapSize;
        for(int x = 0; x < mapSize; x++) {
            for(int y = 0; y < mapSize; y++) {
                this.world.setCell(x, y, depth, accessor.getCell(x, y), false);
            }
        }
    }

    public void generateStartRoom() {
        foreach(MapGenerationData.StartingStructure ss in this.genData.startingStructures) {
            ss.structure.placeIntoWorld(this.world, ss.pos);
        }
    }
}
