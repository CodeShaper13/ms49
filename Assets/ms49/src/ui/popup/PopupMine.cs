using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupMine : PopupWindow {

    public int costPerSquare;

    [SerializeField]
    private Text text = null;

    private World world;

    public override void onAwake() {
        base.onAwake();

        this.world = GameObject.FindObjectOfType<World>();
        this.text.text = this.text.text + this.costPerSquare;
    }

    public override void onUpdate() {
        base.onUpdate();

        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            Vector2Int coords = CameraController.instance.getMousePos();
            int depth = CameraController.instance.currentLayer;

            CellData cell = this.world.getCellState(coords.x, coords.y, depth).data;
            Position pos = new Position(coords.x, coords.y, depth);
            bool isTargeted = this.world.isTargeted(pos);

            if(cell is CellDataMineable) {
                if(!isTargeted && Money.get() >= this.costPerSquare) {
                    this.world.setTargeted(pos, true);
                    Money.remove(costPerSquare);
                }

                else if(isTargeted) {
                    this.world.setTargeted(pos, false);
                    Money.add(costPerSquare);
                }
            }
        }
    }
}
