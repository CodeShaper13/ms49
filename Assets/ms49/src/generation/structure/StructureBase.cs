using System.Collections.Generic;
using UnityEngine;

public abstract class StructureBase : ScriptableObject {

    /// <summary>
    /// Called to generate the structure after all features have been generated.
    /// </summary>
    public abstract void Generate(World world, int depth);

    /// <summary>
    /// Sets a cell in the world and check to see if it is within
    /// the map.  Returns true if the cell could be set.
    /// </summary>
    public bool safeSetCell(World world, Position pos, CellData cell, Rotation r = null) {
        if(world.IsOutOfBounds(pos)) {
            return false;
        }

        // Make sure the cell is not on the edge, it shouldn't overwrite bedrock.
        int edge = world.MapSize - 1;
        if(pos.x == 0 || pos.x == edge || pos.y == 0 || pos.y == edge) {
            return false;
        }

        world.SetCell(pos, cell, r);

        return true;
    }

    public bool safeSetContainer(World world, Position pos, CellData cell, Rotation r, LootTable lootTable) {
        if(this.safeSetCell(world, pos, cell, r)) {
            CellState state = world.GetCellState(pos);
            if(state.behavior is CellBehaviorContainer) {
                CellBehaviorContainer container = (CellBehaviorContainer)state.behavior;

                List<Item> items = lootTable.getRandomItems();
                foreach(Item item in items) {
                    if(item != null) {
                        container.Deposit(item);
                    }
                }
            }
            return true;
        }

        return false;
    }
}
