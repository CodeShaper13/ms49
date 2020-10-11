using UnityEngine;
using fNbt;
using UnityEngine.Tilemaps;

public class CellBehaviorDetectorRail : CellBehavior, IHasData, IRenderTileOverride {

    [SerializeField]
    private TileBase _trippedTileX = null;
    [SerializeField]
    private TileBase _trippedTileY = null;

    public bool isTripped { private get; set; }

    private EntityMinecart tripper;

    //private float tripTimer;

    public override void onUpdate() {
        base.onUpdate();

        //this.tripTimer -= Time.deltaTime;
        //if(this.tripTimer <= 0) {
        //    this.dirty();
        //    this.isTripped = false;
        //}
    }

    public void replaceTiles(ref TileRenderData renderData) {
        if(this.isTripped) {
            renderData.floorOverlayTile =
                this.rotation.axis == EnumAxis.X ?
                this._trippedTileX :
                this._trippedTileY;
        }
    }

    public void readFromNbt(NbtCompound tag) {
        this.isTripped = tag.getBool("isTripped");
        //this.tripTimer = tag.getFloat("tripTimer");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("isTripped", this.isTripped);
        //tag.setTag("tripTimer", this.tripTimer);
    }

    public void setTripped(EntityMinecart cart) {
        if(this.tripper == null || (this.tripper != cart)) {
            this.tripper = cart;
            this.isTripped = !this.isTripped;
            this.dirty();

            CellBehaviorLever.alertNeighborsOfFlip(this);
        }

        /*
        this.tripTimer = 0.1f;
        if(!this.isTripped) {
            this.isTripped = true;
            this.dirty();
        }
        */
    }
}
