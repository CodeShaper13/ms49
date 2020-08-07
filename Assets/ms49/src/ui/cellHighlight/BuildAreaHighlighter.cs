using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildAreaHighlighter : CellHighlightBase {

    [SerializeField]
    private Tilemap tilemap = null;
    [SerializeField]
    private Color transparentColor = Color.white;

    private BuildableBase buildable;
    private PopupBuild popup;

    protected override void Awake() {
        base.Awake();

        this.popup = this.GetComponentInParent<PopupBuild>();
    }

    protected override bool onUpdate(Position pos) {
        // TODO Bad, make better
        if(this.buildable is BuildableTile) {
            BuildableTile b = (BuildableTile)this.buildable;
            for(int x = 0; x < this.buildable.getWidth(); x++) {
                for(int y = 0; y < this.buildable.getHeight(); y++) {
                    CellData data = b.getTile(x, y);
                    if(data != null) {
                        DirectionalTile dt = data.getObjectTile(this.buildable.isRotatable() ? this.popup.rot : null);
                        Vector3Int pod = new Vector3Int(x, y, 0);

                        // Set the tile
                        this.tilemap.SetTile(pod, dt.tile);

                        // Apply rotation
                        if(dt.effect != RotationEffect.NOTHING) {
                            this.tilemap.SetTransformMatrix(pod, dt.getMatrix());
                        }

                        // Make the tile semi transparent.
                        this.tilemap.SetColor(pod, this.transparentColor);
                    }
                }
            }
        }

        return this.buildable.isValidLocation(this.world, pos);
    }

    protected override void onClick(Position pos, bool isValid) {
        if(isValid && Money.get() >= this.buildable.getCost()) {
            Money.remove(this.buildable.getCost());

            this.buildable.placeIntoWorld(world, pos, this.popup.rot);
        }
    }

    public override void hide() {
        base.hide();

        this.tilemap.ClearAllTiles();
    }

    public override void setInvisible() {
        base.setInvisible();

        this.tilemap.ClearAllTiles();
    }

    public void setBuildable(BuildableBase buildable) {
        this.buildable = buildable;
        this.sr.transform.localScale = new Vector3(
            this.buildable.getWidth(),
            this.buildable.getHeight());
    }
}
