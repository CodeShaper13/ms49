using System;
using UnityEngine;

public class FeatureSurfaceTrees : FeatureBase {

    [SerializeField]
    private PrimitiveRndObject[] rndObjects = null;

    public override void Generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        World world = GameObject.FindObjectOfType<World>();
        CellData air = Main.instance.CellRegistry.GetAir();

        for(int x = 0; x < accessor.size; x++) {
            for(int y = 0; y < accessor.size; y++) {
                if(world.IsOutside(new Position(x, y + 1, 0))) {
                    // Outside Tile.
                    CellData cell = air;
                    Rotation rotation = Rotation.UP;
                    foreach(PrimitiveRndObject pro in this.rndObjects) {
                        pro.getRnd(ref cell, ref rotation);
                    }

                    accessor.SetCell(x, y, cell);
                } else {
                    break;
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