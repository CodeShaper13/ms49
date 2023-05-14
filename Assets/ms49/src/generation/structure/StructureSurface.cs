using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Structure", menuName = "MS49/Structure/Surface", order = 1)]
public class StructureSurface : StructureBase {

    [SerializeField]
    private int _surfaceSize = 2;
    [SerializeField]
    private PrimitiveRndObject[] rndObjects = null;

    public override void Generate(World world, int depth) {
        LayerData layerData = world.MapGenerator.GetLayerFromDepth(depth);

        // Generate the inside/outside map
        for(int x = 0; x < world.MapSize; x++) {
            for(int y = 0; y < world.MapSize; y++) {
                Random.State state = Random.state;
                Random.InitState(world.seed);

                float noise = Mathf.PerlinNoise(x * 0.1f, Random.value * 1000);
                float endY = (noise * 7) + this._surfaceSize;

                Random.state = state;

                world.storage.SetOutside(x, y, y < endY);
            }
        }

        for(int x = 0; x < world.MapSize; x++) {
            for(int y = 0; y < world.MapSize; y++) {
                Position pos = new Position(x, y, depth);

                if(world.IsOutside(pos)) {
                    // Outside Tile.

                    CellData cell = null;
                    Rotation rotation = Rotation.UP;
                    if(world.IsOutside(new Position(x, y + 1, depth))) {
                        foreach(PrimitiveRndObject pro in this.rndObjects) {
                            pro.getRnd(ref cell, ref rotation);
                        }

                    }

                    world.SetCell(pos, cell, rotation);

                    // Remove fog from outside
                    world.LiftFog(pos, false);
                } else {
                    // Inside Tile

                    // Remove ores exposed to the surface, but not the bedrock
                    if(x != 0 && x != world.MapSize -1 && world.IsOutside(new Position(x, y - 1, depth))) {
                        world.SetCell(pos, layerData.GetFillCell(world, x, y));
                    }
                }
            }
         }
    }

    [Serializable]
    public class PrimitiveRndObject {

        [SerializeField]
        private CellData cell = null;
        [SerializeField]
        [Range(0, 1)]
        private float chance = 0.5f;
        [SerializeField]
        private bool _randomRotation = false;

        public void getRnd(ref CellData cell, ref Rotation rotation) {
            if(UnityEngine.Random.Range(0f, 1f) < this.chance) {
                cell = this.cell;

                if(this._randomRotation) {
                    rotation = Rotation.ALL[UnityEngine.Random.Range(0, 4)];
                }
            }
        }
    }
}
