using UnityEngine;

public class CellBehaviorBed : CellBehaviorOccupiable {

    [SerializeField]
    private CellData cellBedTop;

    public override void onNeighborChange(CellState triggererCell, Position triggererPos) {
        base.onNeighborChange(triggererCell, triggererPos);

        CellData air = Main.instance.tileRegistry.getAir();
        if(triggererPos == this.pos + Rotation.UP && this.world.getCellState(triggererPos).data == air) {
            // The cell above this behavior was destroyed, remove this cell
            print("removing bottom");
            this.world.setCell(this.pos, air, false);
        }
    }

    public override void onDestroy() {
        base.onDestroy();

        // Remove the top piece of bed
        CellData air = Main.instance.tileRegistry.getAir();
        Position pos1 = this.pos + Rotation.UP;
        if(this.world.getCellState(pos1).data != air) {
            print("removing top piece");
            this.world.setCell(pos1, air, false);
        }
    }
}
