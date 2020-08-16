using UnityEngine;

public class DemoHighlighter : CellHighlightBase {

    [SerializeField]
    private PopupDemo popup = null;
    [SerializeField]
    private IntVariable money = null;
    [SerializeField]
    private GameObject particlePrefab = null;

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
            if(this.money.value >= this.popup.getDemoCost()) {
                this.money.value -= cost;
                this.world.setCell(
                    pos.x,
                    pos.y,
                    pos.depth,
                    Main.instance.tileRegistry.getAir());

                this.world.particles.spawn(pos, particlePrefab);
            }
        }
    }
}