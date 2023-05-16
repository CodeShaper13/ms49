using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class EntityTruck : EntityBase {

    private const int TRUCK_LENGTH = 3;

    [SerializeField, Required]
    private Animator _animator = null;
    [SerializeField, Required]
    private Inventory _inventory = null;
    [SerializeField, Required]
    private PathfindingAgent _agent = null;

    [Space]

    [SerializeField]
    private CellData _depositPoint = null;

    private CellData[] path = new CellData[TRUCK_LENGTH];
    
    public bool isDriving { get; private set; }

    public Inventory Inventory => this._inventory;

    private void Start() {
        this.rotation = Rotation.LEFT;
        this.StopDriving();
    }

    public override void Update() {
        base.Update();

        if(!this.isDriving && this.Inventory.IsFull) {
            this.StartDriving();
        }
    }

    public override void OnRightClick() {
        base.OnRightClick();

        PopupTruck popup = Main.instance.findPopup<PopupTruck>();
        if(popup != null) {
            popup.open();
            popup.SetTruck(this);
        }
    }

    public void DriveOffMap() {
        List<CellBehaviorTruckExit> list = this.world.GetAllBehaviors<CellBehaviorTruckExit>((behaviour) => !behaviour.isEntrance);

    }

    public void StartDriving() {
        this.isDriving = true;

        // Restore road.
        for(int i = 0; i < TRUCK_LENGTH; i++) {
            Position pos = this.Func(i);

            this.world.SetCell(pos, this.path[i], true);
        }
    }

    public void StopDriving() {
        this.isDriving = false;

        // Place deposit points.
        for(int i = 0; i < TRUCK_LENGTH; i++) {
            Position pos = this.Func(i);

            this.path[i] = this.world.GetCellState(pos)?.data;
            this.world.SetCell(pos, this._depositPoint, true);
        }
    } 

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

    private Position Func(int index) {
        return this.position - this.rotation + (this.rotation * index);
    }
}