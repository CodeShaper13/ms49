using UnityEngine;

public class DemoHighlighter : CellHighlightBase {

    [SerializeField]
    private PopupDemo popup = null;

    protected override bool onUpdate(Position pos) {   
        if(!this.world.isOutOfBounds(pos)) {
            if(this.world.getCellState(pos).data.isDestroyable) {
                return true;
            }
        }
        return false;
    }

    protected override void onClick(Position pos, bool isValid) {
        if(isValid) {
            int cost = this.popup.getDemoCost();
            if(Money.get() >= this.popup.getDemoCost()) {
                Money.remove(cost);
                this.world.setCell(
                    pos.x,
                    pos.y,
                    pos.depth,
                    Main.instance.tileRegistry.getAir());
            }
        }
    }
}