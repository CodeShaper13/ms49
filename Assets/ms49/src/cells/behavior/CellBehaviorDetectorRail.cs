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

    private void Update() {
        //this.tripTimer -= Time.deltaTime;
        //if(this.tripTimer <= 0) {
        //    this.dirty();
        //    this.isTripped = false;
        //}
        if(this.tripper != null && this.tripper.position != this.pos) {
            this.tripper = null;
        }
    }

    public void replaceTiles(ref TileRenderData renderData) {
        if(this.isTripped) {
            renderData.floorOverlayTile =
                this.rotation.axis == EnumAxis.X ?
                this._trippedTileX :
                this._trippedTileY;
        }
    }

    public void ReadFromNbt(NbtCompound tag) {
        this.isTripped = tag.getBool("isTripped");
        //this.tripTimer = tag.getFloat("tripTimer");
    }

    public void WriteToNbt(NbtCompound tag) {
        tag.setTag("isTripped", this.isTripped);
        //tag.setTag("tripTimer", this.tripTimer);
    }

    public void SetTripped(EntityMinecart cart) {
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
