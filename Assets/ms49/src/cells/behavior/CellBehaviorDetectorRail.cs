using UnityEngine;
using fNbt;
using UnityEngine.Tilemaps;

public class CellBehaviorDetectorRail : CellBehavior, IHasData, IRenderTileOverride {

    [SerializeField]
    private TileBase trippedTileX = null;
    [SerializeField]
    private TileBase trippedTileY = null;

    public bool isTripped { private get; set; }

    private float tripTimer;

    public override void onUpdate() {
        base.onUpdate();

        this.tripTimer -= Time.deltaTime;
        if(this.tripTimer <= 0) {
            this.dirty();
            this.isTripped = false;
        }
    }

    public void replaceTiles(ref TileRenderData renderData) {
        if(this.isTripped) {
            renderData.floorOverlayTile = this.rotation.axis == EnumAxis.X ? this.trippedTileX : this.trippedTileY;
        }
    }

    public void readFromNbt(NbtCompound tag) {
        this.isTripped = tag.getBool("tripped");
        this.tripTimer = tag.getFloat("tripTimer");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("tripped", this.isTripped);
        tag.setTag("tripTimer", this.tripTimer);
    }

    public void setTripped() {
        this.tripTimer = 0.1f;
        if(!this.isTripped) {
            this.isTripped = true;
            this.dirty();
        }
    }
}
