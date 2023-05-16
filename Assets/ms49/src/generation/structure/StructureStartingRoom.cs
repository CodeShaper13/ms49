using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "StartingRoom", menuName = "MS49/Structure/Starting Room", order = 1)]
public class StructureStartingRoom : StructureBase {

    [SerializeField]
    private Prebuilt[] _areas = null;

    [SerializeField]
    private CellData _cellRoad = null;
    [SerializeField]
    private CellData _cellGate = null;
    [SerializeField]
    private GameObject _prefabDumptruck = null;

    public override void Generate(World world, int depth) {

        PlotManager pm = world.plotManager;
        int structureX = Random.Range(
            5,
            pm.plotDiameter - 5) + ((pm.plotCount / 2) * pm.plotDiameter);

        foreach(Prebuilt p in this._areas) {
            Random.InitState(world.seed);

            Position pos = new Position(structureX + p.position.x, p.position.y, depth);
            p.area.placeIntoWorld(world, pos);
        }

        // Create road.
        const int ROAD_LENGTH = 5;
        const int ROAD_Y = 3;

        for(int i = -ROAD_LENGTH; i <= ROAD_LENGTH; i++) {
            if(Mathf.Abs(i) == ROAD_LENGTH) {
                for(int j = ROAD_Y; j >= 0; j--) {
                    world.SetCell(structureX + i, j, 0, j == 0 ? this._cellGate : this._cellRoad);
                }
            } else {
                world.SetCell(structureX + i, ROAD_Y, 0, this._cellRoad);
            }

            if(i == 0) {
                world.entities.Spawn(
                    new Position(structureX + i, ROAD_Y, 0),
                    Main.instance.EntityRegistry.GetIdOfElement(this._prefabDumptruck));
            }
        }
    }

    [Serializable]
    private class Prebuilt {

        public PrebuiltArea area = null;
        public Vector2Int position = Vector2Int.zero;
    }
}
