﻿using UnityEngine;

public class DemoHighlighter : CellHighlightBase {

    [SerializeField]
    private PopupDemo popup = null;
    [SerializeField]
    private IntVariable money = null;
    [SerializeField]
    private GameObject particlePrefab = null;

    private EntityBase destroyableEntity;

    protected override bool onUpdate(Position pos) {
        this.destroyableEntity = null;

        if(!this.world.isOutOfBounds(pos)) {
            // Check if the mouse is over an Entity that is destroyable.
            RaycastHit2D hit = CameraController.instance.getMouseOver();
            if(hit.collider != null) {
                EntityBase e = hit.transform.GetComponent<EntityBase>();
                if(e.isDestroyable()) {
                    this.destroyableEntity = e;
                    return true;
                }
            }

            // Check if the mouse is over a Destroyable cell.
            if(this.world.getCellState(pos).data.isDestroyable && this.world.plotManager.isOwned(pos)) {
                return true;
            }
        }
        return false;
    }

    protected override void onClick(Position pos, bool isValid) {
        if(isValid) {
            int cost = this.popup.getDemoCost();
            bool inCreative = CameraController.instance.inCreativeMode;
            if((inCreative || this.money.value >= this.popup.getDemoCost()) && this.world.plotManager.isOwned(pos)) {
                if(!inCreative) {
                    this.money.value -= cost;
                }

                Vector2 particlePos;
                if(this.destroyableEntity != null) {
                    particlePos = this.destroyableEntity.worldPos;
                    this.world.entities.remove(this.destroyableEntity);
                } else {
                    particlePos = pos.center;
                    this.world.setCell(pos, null);
                    this.world.tryCollapse(pos);
                }

                this.world.particles.spawn(particlePos, pos.depth, particlePrefab);
            }
        }
    }
}