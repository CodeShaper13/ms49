using UnityEngine;
using UnityEngine.EventSystems;

public class PopupMine : PopupWorldReference {

    [SerializeField]
    private AudioSource audioCellToggle = null;

    protected override void onUpdate() {
        base.onUpdate();

        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            Position pos = CameraController.instance.getMousePos();

            CellData cell = this.world.getCellState(pos).data;
            bool isTargeted = this.world.targetedSquares.isTargeted(pos);

            if(cell is CellDataMineable && this.world.plotManager.isOwned(pos)) {
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
                    if(!isTargeted) {
                        this.world.targetedSquares.setTargeted(pos, true);
                        this.playSfx();
                    }
                    else if(isTargeted) {
                        this.world.targetedSquares.setTargeted(pos, false);
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
