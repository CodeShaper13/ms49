using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "MS49/Structure/Dungeon", order = 1)]
public class StructureDungeon : StructureBase {

    [SerializeField]
    private CellData _chest = null;
    [SerializeField]
    private CellData _wall = null;
    [SerializeField]
    private LootTable _chestLootTable = null;


    [Space]

    [SerializeField]
    private int _dungeonsPerLayer = 50;
    [SerializeField, MinMaxSlider(1, 10)]
    private Vector2Int _size = new Vector2Int(3, 6);

    public override void generate(World world, int depth) {
        for(int i = 0; i < this._dungeonsPerLayer; i++) {
            const int edgeBuffer = 5;
            int xPos = Random.Range(edgeBuffer, world.mapSize + edgeBuffer);
            int yPos = Random.Range(edgeBuffer, world.mapSize + edgeBuffer);

            int xSize = this.func();
            int ySize = this.func();

            int edgeAirCount = 0;
            for(int x = 0; x < xSize; x++) {
                for(int y = 0; y < ySize; y++) {
                    int xEnd = xSize - 1;
                    int yEnd = ySize - 1;
                    if(x == 0 || y == 0 || x == xEnd || y == yEnd) {
                        Position pos = new Position(x + xPos, y + yPos, depth);
                        if(!world.isOutOfBounds(pos)) {
                            CellData data = world.getCellState(pos).data;
                            if(data.canBuildOver || data.isDestroyable) {
                                edgeAirCount ++;
                            }
                        }
                    }
                }
            }

            if(edgeAirCount >= 1 && edgeAirCount <= 4) {
                // Generate room.
                for(int x = 0; x < xSize; x++) {
                    for(int y = 0; y < ySize; y++) {
                        Position pos = new Position(x + xPos, y + yPos, depth);
                        if(!world.isOutOfBounds(pos)) {
                            CellData oldCell = world.getCellState(pos).data;
                            int xEnd = xSize - 1;
                            int yEnd = ySize - 1;
                            bool edge = x == 0 || y == 0 || x == xEnd || y == yEnd;

                            if(edge) {
                                if(oldCell is CellDataMineable || (x == 0 && y == 0) || (x == 0 && y == yEnd) || (x == xEnd && y == 0) || (x == xEnd && y == yEnd)) {
                                    world.setCell(pos, this._wall, Rotation.ALL[Random.Range(0, 3)]);
                                }
                            } else {
                                if(oldCell is CellDataMineable || Random.value < 0.5f) {
                                    world.setCell(pos, null);
                                }
                            }                         
                        }
                    }
                }

                this.safeSetContainer(
                    world,
                    new Position(xPos + 2, yPos + 2, depth),
                    this._chest,
                    Rotation.UP,
                    this._chestLootTable);
            }
        }
    }

    private int func() {
        return Random.Range(this._size.x, this._size.y);
    }
}
