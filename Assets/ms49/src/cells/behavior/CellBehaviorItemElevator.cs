using UnityEngine;
using fNbt;

public class CellBehaviorItemElevator : CellBehaviorContainer {

    [SerializeField, Min(0)]
    private float itemTransferSpeed = 1f;

    private float transferTimer;

    public bool IsGoingUp => this.rotation.axis == EnumAxis.Y;

    public override void OnCreate(World world, CellState state, Position pos) {
        base.OnCreate(world, state, pos);
    }

    public void Update() {
        if(Pause.IsPaused) {
            return;
        }

        this.transferTimer -= Time.deltaTime;

        if(this.transferTimer <= 0 && !this.IsEmpty) {
            int depthChange = this.IsGoingUp ? -1 : 1;
            CellBehaviorItemElevator behavior = this.world.GetCellBehavior<CellBehaviorItemElevator>(this.pos.Add(0, 0, depthChange), true);
            if(behavior != null && !behavior.IsFull) {
                behavior.Deposit(this.PullItem());
                this.transferTimer = this.itemTransferSpeed;
            }
        }
    }

    public override void ReadFromNbt(NbtCompound tag) {
        base.ReadFromNbt(tag);

        this.transferTimer = tag.GetFloat("transferTimer");
    }

    public override void WriteToNbt(NbtCompound tag) {
        base.WriteToNbt(tag);

        tag.SetTag("transferTimer", this.transferTimer);
    }

    private enum EnumDirection {
        UP = 0,
        DOWN = 1,
    }
}
