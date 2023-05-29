using UnityEngine;
using Random = UnityEngine.Random;

public class FeatureCliffline : FeatureBase {

    [SerializeField, Min(0)]
    private int _surfaceSize = 2;

    public override void Generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        int size = accessor.size;
        World world = GameObject.FindObjectOfType<World>();
        
        // Generate the inside/outside map.
        for(int x = 0; x < size; x++) {
            for(int y = 0; y < size; y++) {
                Random.State state = Random.state;
                Random.InitState(world.seed);

                float noise = Mathf.PerlinNoise(x * 0.1f, Random.value * 1000);
                float endY = (noise * 7) + this._surfaceSize;

                Random.state = state;

                world.storage.SetOutside(x, y, y < endY);
            }
        }
        CellData air = Main.instance.CellRegistry.GetAir();

        for(int x = 0; x < world.MapSize; x++) {
            for(int y = 0; y < world.MapSize; y++) {
                Position pos = new Position(x, y, 0);

                if(world.IsOutside(pos)) {
                    // Remove fog from outside
                    world.LiftFog(pos, false);
                    accessor.SetCell(x, y, air);
                }
                else {
                    // Remove ores exposed to the surface, but not the bedrock
                    if(x != 0 && x != size - 1 && world.IsOutside(new Position(x, y - 1, 0))) {
                        accessor.SetCell(x, y, layerData.GetFillCell(world, x, y));
                    }
                }
            }
        }
    }
}