using UnityEngine;

public class CellBehaviorDepositPoint : CellBehaviorContainer, IHasData, IContainer {

    [SerializeField, Tooltip("Master points do not store items.  Instead they \"sell\" the items.")]
    private bool _isMaster = false;

    public bool isMaster => this._isMaster;
    public override bool isFull => this._isMaster ? false : this.inventory.isFull();
    public override bool isEmpty => this._isMaster ? true : this.inventory.isEmpty();

    private void OnValidate() {
        if(this._isMaster) {
            this.inventorySize = 0;
        }
    }

    public override void onRightClick() {
        if(!this.isMaster) {
            base.onRightClick();
        }
    }

    public override void deposit(Item item) {
        if(this._isMaster) {
            this.world.economy.sellItem(item);
        }
        else {
            base.deposit(item);
        }
    }
}
