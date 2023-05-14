using UnityEngine;

// Unused
public class StructureScatteredCell : StructureBase {

    [SerializeField]
    private CellData _cell = null;
    [SerializeField, Range(0, 1)]
    private float _chance = 0.5f;
    [SerializeField]
    private EnumLocation _location = EnumLocation.Both;
    [SerializeField, Tooltip("If true, the Cell will have a random rotation.")]
    private bool _randomlyRotate = false;

    public override void Generate(World world, int depth) {
        for(int x = 0; x < world.MapSize; x++) {
            for(int y = 0; y < world.MapSize; y++) {
                if(Random.Range(0f, 1f) < this._chance) {
                    Rotation rot;

                    if(this._randomlyRotate) {
                        rot = Rotation.ALL[Random.Range(0, 4)];
                    } else {
                        rot = Rotation.UP;
                    }

                    world.SetCell(
                        new Position(x, y, depth),
                        this._cell);
                }
            }
        }
    }

    private enum EnumLocation {
        Inside = 0,
        Outside = 1,
        Both = 2,
    }
}