using UnityEngine;
using fNbt;

public class CellBehaviorItemElevator : CellBehaviorContainer {

    [SerializeField, Min(0)]
    private float itemTransferSpeed = 1f;

    private float transferTimer;

    public override void onCreate(World world, CellState state, Position pos) {
        base.onCreate(world, state, pos);
    }

    public override void onUpdate() {
        base.onUpdate();

        this.transferTimer -= Time.deltaTime;

        if(this.transferTimer <= 0 && !this.isEmpty) {
            int depthChange = this.isGoingUp() ? -1 : 1;
            CellBehaviorItemElevator behavior = this.world.getBehavior<CellBehaviorItemElevator>(this.pos.add(0, 0, depthChange));
            if(behavior != null && !behavior.isFull) {
                behavior.deposit(this.pullItem());
                this.transferTimer = this.itemTransferSpeed;
            }
        }
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.transferTimer = tag.getFloat("transferTimer");
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("transferTimer", this.transferTimer);
    }

    public bool isGoingUp() {
        return this.rotation.axis == EnumAxis.Y;
    }

    private enum EnumDirection {
        UP = 0,
        DOWN = 1,
    }
}
