using UnityEngine;

public class EntityTruck : EntityBase {

    [SerializeField]
    private Animator _animator = null;
    [SerializeField]
    private Inventory _inventory = null;

    /// <summary>
    /// Sells all of the Trucks contents.
    /// </summary>
    private void SellContents() {
        if(this._inventory != null) {
            for(int i = 0; i < this._inventory.Size; i++) {
                this.world.economy.sellItem(this._inventory[i]);
            }
        }
    }
}