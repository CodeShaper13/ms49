﻿using UnityEngine;

public class BuildAreaHighlighter : CellHighlightBase {

    [SerializeField]
    private Color transparentColor = Color.white;
    [SerializeField]
    public CellData buildSiteCell = null;
    [SerializeField]
    private PopupBuild popup = null;
    [SerializeField]
    public CellTilemapRenderer cellRenderer;
    [SerializeField]
    public SpriteRenderer tileSr = null;

    private BuildableBase buildable;

    protected override bool onUpdate(Position pos) {
        bool isValid = this.buildable.isValidLocation(this.world, pos, this.popup.rot);

        if(isValid) {
            if(this.buildable is ISpritePreview) {
                this.cellRenderer.gameObject.SetActive(false);
                this.tileSr.gameObject.SetActive(true);

                this.tileSr.sprite = ((ISpritePreview)this.buildable).getPreviewSprite(this.world, pos);

            } else if(this.buildable is BuildableTile) {
                this.cellRenderer.gameObject.SetActive(true);
                this.tileSr.gameObject.SetActive(false);

                this.cellRenderer.initializedRenderer(
                    Mathf.Max(this.buildable.getHighlightWidth(), this.buildable.getHighlightHeight()),
                    null,
                    null,
                    (x, y) => {
                        CellData data;
                        if(this.buildable is BuildableMultiCellTile) {
                            data = ((BuildableMultiCellTile)this.buildable).getCellAt(x, y);
                        } else {
                            data = ((BuildableTile)this.buildable).cell;
                        }

                        if(data == null) {
                            return null;
                        } else {
                            return new CellState(data, null, this.buildable.isRotatable() ? this.popup.rot : Rotation.UP);
                        }
                    },
                    null);

                this.cellRenderer.totalRedraw = true;
            }
        } else {
            this.cellRenderer.gameObject.SetActive(false);
            this.tileSr.gameObject.SetActive(false);
        }

        if(isValid && this.world.money.value >= this.buildable.cost) {
            this.setValidColor();
        } else {
            this.setInvalidColor();
        }

        return isValid;
    }

    protected override void onClick(Position pos, bool isValid) {
        bool inCreative = CameraController.instance.inCreativeMode;

        if(isValid && (inCreative || world.money.value >= this.buildable.cost)) {
            if(!inCreative) {
                this.world.money.value -= this.buildable.cost;
            }

            this.buildable.placeIntoWorld(world, this, pos, this.popup.rot);
        }
    }

    public override void hide() {
        base.hide();

        this.cellRenderer.clear();
    }

    public override void setInvisible() {
        base.setInvisible();

        this.cellRenderer.clear();
    }

    public void setBuildable(BuildableBase buildable) {
        this.buildable = buildable;
        this.sr.transform.localScale = new Vector3(
            this.buildable.getHighlightWidth(),
            this.buildable.getHighlightHeight());
    }
}
