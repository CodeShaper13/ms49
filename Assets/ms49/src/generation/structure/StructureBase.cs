using System.Collections.Generic;
using UnityEngine;

public abstract class StructureBase : ScriptableObject {

    /// <summary>
    /// Called to generate the structure after all features have been generated.
    /// </summary>
    public abstract void generate(World world, int depth);

    /// <summary>
    /// Sets a cell in the world and check to see if it is within
    /// the map.  Returns true if the cell could be set.
    /// </summary>
    public bool safeSetCell(World world, Position pos, CellData cell, Rotation r = null) {
        if(world.isOutOfBounds(pos)) {
            return false;
        }

        // Make sure the cell is not on the edge, it shouldn't overwrite bedrock.
        int edge = world.mapSize - 1;
        if(pos.x == 0 || pos.x == edge || pos.y == 0 || pos.y == edge) {
            return false;
        }

        world.setCell(pos, cell, r);

        return true;
    }

    public bool safeSetContainer(World world, Position pos, CellData cell, Rotation r, LootTable lootTable) {
        if(this.safeSetCell(world, pos, cell, r)) {
            CellState state = world.getCellState(pos);
            if(state.behavior is AbstractBehaviorContainer) {
                AbstractBehaviorContainer container = (AbstractBehaviorContainer)state.behavior;

                List<Item> items = lootTable.getRandomItems();
                foreach(Item item in items) {
                    if(item != null) {
                        container.deposit(item);
                    }
                }
            }
            return true;
        }

        return false;
    }
}
