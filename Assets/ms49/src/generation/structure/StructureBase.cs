using UnityEngine;

public abstract class StructureBase : ScriptableObject {

    public abstract void placeIntoWorld(World world, Position pos);

    public void safeSetCell(World world, Position pos, CellData cell, Rotation r = null) {
        if(world.isOutOfBounds(pos)) {
            return;
        }

        world.setCell(pos, cell, r);
    }
}
