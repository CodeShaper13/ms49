using UnityEngine;

public class StructureTruckRoads : StructureBase {

    [SerializeField]
    private CellData _cellRoad = null;
    [SerializeField]
    private CellData _cellGate = null;
    [SerializeField]
    private CellData _cellStopSign = null;
    [SerializeField]
    private GameObject _prefabDumptruck = null;

    [Space]

    [SerializeField, Min(0)]
    private int _roadY = 0;

    public override void GenerateStructure(World world, int depth) {
        Position pos = new Position((world.MapSize / 2) - 2, this._roadY, depth);

        // Road.
        for(int x = 0; x < world.MapSize; x++) {
            world.SetCell(new Position(x, this._roadY, depth), this._cellRoad);
        }

        // Gates.
        world.SetCell(0, this._roadY, 0, this._cellGate);
        world.SetCell(world.MapSize - 1, this._roadY, 0, this._cellGate);

        // Stop sign.
        world.SetCell(pos + Rotation.UP + Rotation.LEFT * 2, this._cellStopSign);

        // Truck.
        world.entities.Spawn(
            pos,
            Main.instance.EntityRegistry.GetIdOfElement(this._prefabDumptruck));
    }
}