using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "StartingRoom", menuName = "MS49/Structure/Starting Room", order = 1)]
public class StructureStartingRoom : StructureBase {

    [SerializeField]
    private Prebuilt[] _areas = null;

    public override void generate(World world, int depth) {

        PlotManager pm = world.plotManager;
        int structureX = Random.Range(
            5,
            pm.plotDiameter - 5) + ((pm.plotCount / 2) * pm.plotDiameter);

        foreach(Prebuilt p in this._areas) {
            Random.InitState(world.seed);

            Position pos = new Position(structureX + p.position.x, p.position.y, depth);
            p.area.placeIntoWorld(world, pos);
        }
    }

    [Serializable]
    private class Prebuilt {

        public PrebuiltArea area = null;
        public Vector2Int position = Vector2Int.zero;
    }
}
