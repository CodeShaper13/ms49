using System;
using UnityEngine;

public class BuildAreaHighlighter : CellHighlightBase {

    [SerializeField]
    private Color transparentColor = Color.white;
    [SerializeField]
    public CellData buildSiteCell = null;
    [SerializeField]
    private PopupBuild popup = null;

    private BuildableBase buildable;

    public CellTilemapRenderer cellRenderer;

    protected override bool onUpdate(Position pos) {
        bool isValid = this.buildable.isValidLocation(this.world, pos);

        if(this.buildable is BuildableTile && isValid) {
            BuildableTile b = (BuildableTile)this.buildable;

            this.cellRenderer.gameObject.SetActive(true);

            this.cellRenderer.mapSize = Math.Max(this.buildable.getWidth(), this.buildable.getHeight());

            this.cellRenderer.cellStateGetterFunc = (x, y) => {
                CellData data = b.getTile(x, y);
                if(data == null) {
                    return null;
                } else {
                    return new CellState(data, null, this.buildable.isRotatable() ? this.popup.rot : null);
                }
            };
            this.cellRenderer.totalRedraw = true;

        } else {
            this.cellRenderer.gameObject.SetActive(false);
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

        //this.cellRenderer.clear();
    }

    public override void setInvisible() {
        base.setInvisible();

        //this.cellRenderer.clear();
    }

    public void setBuildable(BuildableBase buildable) {
        this.buildable = buildable;
        this.sr.transform.localScale = new Vector3(
            this.buildable.getWidth(),
            this.buildable.getHeight());
    }
}
