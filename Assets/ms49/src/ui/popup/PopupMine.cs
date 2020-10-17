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

            bool valid = cell is CellDataMineable || (cell == Air.get && this.world.isCoveredByFog(pos));
            if(valid) {
                if(CameraController.instance.inCreativeMode) {
                    // Instantly remove.
                    this.world.setCell(pos, null, true);
                    this.world.liftFog(pos);
                    this.world.tryCollapse(pos);
                    this.world.targetedSquares.setTargeted(pos, false);
                    this.playSfx();
                }
                else {
                    if(this.world.plotManager.isOwned(pos)) {
                        // Mark to be removed.
                        if(this.world.targetedSquares.isTargeted(pos)) {
                            this.world.targetedSquares.setTargeted(pos, false);
                        } else {
                            this.world.targetedSquares.setTargeted(pos, true);
                        }

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
