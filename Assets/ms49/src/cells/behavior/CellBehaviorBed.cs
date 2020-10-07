using UnityEngine;

public class CellBehaviorBed : CellBehaviorOccupiable {

    public override void onNeighborChange(CellState triggererCell, Position triggererPos) {
        base.onNeighborChange(triggererCell, triggererPos);

        CellData air = Main.instance.tileRegistry.getAir();
        if(triggererPos == this.pos + Rotation.DOWN && this.world.getCellState(triggererPos).data == air) {
            // The cell below this behavior was destroyed, remove this cell
            print("removing top");
            this.world.setCell(this.pos, air, false);
        }
    }

    public override void onDestroy() {
        base.onDestroy();

        // Remove the bottom of bed
        CellData air = Main.instance.tileRegistry.getAir();
        Position pos1 = this.pos + Rotation.DOWN;
        if(this.world.getCellState(pos1).data != air) {
            print("removing bottom piece");
            this.world.setCell(pos1, air, false);
        }
    }
}
