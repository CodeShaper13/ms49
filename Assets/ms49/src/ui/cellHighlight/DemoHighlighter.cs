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
            EntityBase e = CameraController.instance.getMouseOver();
            if(e != null && e.isDestroyable()) {
                this.destroyableEntity = e;

                this.setValidColor();
                return true;
            }

            // Check if the mouse is over a Destroyable cell.
            if(this.world.getCellState(pos).data.isDestroyable && (this.world.plotManager.isOwned(pos) || CameraController.instance.inCreativeMode)) {
                this.setValidColor();
                return true;
            }
        }

        this.setInvalidColor();
        return true;
    }

    protected override void onClick(Position pos, bool isValid) {
        if(isValid) {
            int cost = this.popup.getDemoCost();
            bool inCreative = CameraController.instance.inCreativeMode;
            if((inCreative || (this.money.value >= this.popup.getDemoCost()) && this.world.plotManager.isOwned(pos))) {
                if(!inCreative) {
                    this.money.value -= cost;
                }

                Vector2 particlePos;
                if(this.destroyableEntity != null) {
                    particlePos = this.destroyableEntity.worldPos;
                    this.world.entities.remove(this.destroyableEntity);
                } else {
                    // Add to the destroyed stat.
                    CellData cell = this.world.getCellState(pos).data;
                    StatisticInt stat = this.world.statManager.getCellDestroyedStat(cell);
                    if(stat != null) {
                        stat.increase(1);
                    } else {
                        print("error");
                    }

                    particlePos = pos.center;

                    // Remove the Cell.
                    this.world.setCell(pos, null);
                    this.world.tryCollapse(pos);
                }

                this.world.particles.spawn(particlePos, pos.depth, particlePrefab);
            }
        }
    }
}