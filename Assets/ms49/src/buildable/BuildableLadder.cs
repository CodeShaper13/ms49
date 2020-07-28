using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "MS49/Structure Ladder", order = 1)]
public class BuildableLadder : BuildableTile {

    [SerializeField]
    private CellData ladderBottom = null;

    public override bool isValidLocation(World world, Position pos) {
        // Make sure this isn't the bottom of the map.
        if(pos.depth >= world.storage.layerCount - 1) {
            return false;
        }

        bool isTopClear = base.isValidLocation(world, pos);
        CellData data = world.getCellState(pos.x, pos.y, pos.depth + 1).data;
        bool isBottomClear = data.canBuildOver || data is CellDataMineable;
        return isTopClear && isBottomClear;
    }

    public override void placeIntoWorld(World world, Position pos, Rotation rotation) {
        // Place the normal structure, the top.
        base.placeIntoWorld(world, pos, rotation);

        // Place the bottom.
        Position pos1 = pos.add(0, 0, 1);
        world.setCell(pos1, this.ladderBottom);
        world.liftFog(pos1);
    }
}
