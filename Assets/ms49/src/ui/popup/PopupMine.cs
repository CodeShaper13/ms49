using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupMine : PopupWorldReference {

    public int costPerSquare;

    [SerializeField]
    private Text text = null;
    [SerializeField]
    private IntVariable money = null;
    [SerializeField]
    private AudioSource audioCellToggle = null;

    protected override void initialize() {
        base.initialize();

        this.text.text = string.Format(this.text.text, this.costPerSquare);
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            Position pos = CameraController.instance.getMousePos();

            CellData cell = this.world.getCellState(pos).data;
            bool isTargeted = this.world.targetedSquares.isTargeted(pos);

            if(cell is CellDataMineable) {
                if(CameraController.instance.inCreativeMode) {
                    this.world.setCell(pos, Main.instance.tileRegistry.getAir(), true);
                    this.world.liftFog(pos);
                    this.world.tryCollapse(pos);

                    if(isTargeted) {
                        this.world.targetedSquares.setTargeted(pos, false);
                    }

                    this.playSfx();
                }
                else {
                    if(!isTargeted && this.money.value >= this.costPerSquare) {
                        this.world.targetedSquares.setTargeted(pos, true);
                        this.money.value -= this.costPerSquare;
                        this.playSfx();
                    }
                    else if(isTargeted) {
                        this.world.targetedSquares.setTargeted(pos, false);
                        this.money.value += this.costPerSquare;
                        this.playSfx();
                    }
                }
            }
        }
    }

    private void playSfx() {
        if(this.audioCellToggle != null) {
            this.audioCellToggle.Play();
        }
    }
}
