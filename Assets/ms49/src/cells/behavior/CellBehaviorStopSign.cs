using UnityEngine;

public class CellBehaviorStopSign : CellBehavior {

    [SerializeField]
    private CellData _road = null;

    /// <summary>
    /// The Truck at, or going to, this sign.
    /// </summary>
    private EntityTruck truck;

    public bool IsOccupied => this.truck = null;
    public Position TruckStopPoint => this.pos + Rotation.DOWN;

    public bool IsValidStopSign() {
        Position pos = this.TruckStopPoint;

        if(this.world.IsOutOfBounds(pos)) {
            return false;
        }

        return this.world.GetCellState(pos).data == this._road;
    }

    public override string GetTooltipText() {
        return this.IsValidStopSign() ? null : "Stop Sign need a Road in front of it!";
    }
}