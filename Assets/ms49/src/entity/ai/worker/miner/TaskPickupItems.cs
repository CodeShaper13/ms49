using UnityEngine;

public class TaskPickupItems : TaskMovement<EntityWorker> {

    [SerializeField]
    private MinerMetaData _minerData = null;
    [SerializeField]
    private CellData[] _ignoredItemOn = new CellData[0];

    private EntityItem itemEntity;

    public override bool continueExecuting() {
        if(this.itemEntity == null) {
            return false;
        }

        if(this._minerData.heldItem != null) {
            return false;
        }

        return !this.isOnBlockedCell(this.itemEntity);
    }

    public override bool shouldExecute() {
        if(this._minerData.heldItem == null) {
            foreach(EntityBase e in this.owner.world.entities.list) {
                if(e is EntityItem && this.owner.world.GetCellState(e.position).data.IsWalkable && !this.isOnBlockedCell(e)) {
                    if(this.calculateAndSetPath(e.position)) {
                        this.itemEntity = (EntityItem)e;
                        return true;
                    }                
                }
            }
        }

        return false;
    }

    public override void onDestinationArive() {
        base.onDestinationArive();

        this._minerData.heldItem = this.itemEntity.item;

        this.owner.world.entities.Remove(this.itemEntity);
    }

    public override void onTaskStop() {
        base.onTaskStop();

        this.itemEntity = null;
    }

    private bool isOnBlockedCell(EntityBase entity) {
        CellData cell = this.owner.world.GetCellState(entity.position).data;
        foreach(CellData ignoreCell in this._ignoredItemOn) {
            if(ignoreCell == cell) {
                return true;
            }
        }

        return false;
    }
}
