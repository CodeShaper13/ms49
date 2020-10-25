using UnityEngine;
using UnityEngine.EventSystems;

public class PopupMine : PopupWorldReference {

    [SerializeField]
    private AudioSource audioCellToggle = null;

    protected override void onUpdate() {
        base.onUpdate();

        bool leftBtn = Input.GetMouseButtonDown(0);
        bool rightBtn = Input.GetMouseButtonDown(1);

        if((rightBtn || leftBtn) && !EventSystem.current.IsPointerOverGameObject()) {
            Position pos = CameraController.instance.getMousePos();

            CellData cell = this.world.getCellState(pos).data;

            bool valid = cell is CellDataMineable || (cell == Air.get && this.world.isCoveredByFog(pos));
            if(valid) {
                if(CameraController.instance.inCreativeMode) {
                    // Instantly remove.
                    this.world.setCell(pos, null, true);
                    this.world.liftFog(pos);
                    this.world.tryCollapse(pos);
                    this.world.targetedSquares.stopTargeting(pos);
                    this.playSfx();
                }
                else {
                    if(this.world.plotManager.isOwned(pos)) {
                        if(this.world.targetedSquares.isTargeted(pos)) {
                            if(!rightBtn) {
                                this.world.targetedSquares.stopTargeting(pos);
                            } else {
                                if(this.world.targetedSquares.isPriority(pos)) {
                                    this.world.targetedSquares.stopTargeting(pos);
                                } else {
                                    this.world.targetedSquares.stopTargeting(pos);
                                    this.world.targetedSquares.startTargeting(pos, true);
                                }
                            }
                        } else {
                            // Make sure this won't leave too big of an open spot // TODO
                            this.world.targetedSquares.startTargeting(pos, rightBtn);
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
