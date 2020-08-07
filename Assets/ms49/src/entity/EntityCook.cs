using fNbt;

public class EntityCook : EntityWorker {

    public CellBehaviorTable.EnumPlateState plateState { get; set; } = CellBehaviorTable.EnumPlateState.CLEAN;

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.aiManager.addTask(5, new TaskMakeFood(this, this.moveHelper));
        this.aiManager.addTask(8, new TaskCollectDishes(this, this.moveHelper));
        this.aiManager.addTask(9, new TaskWashPlate(this, this.moveHelper));
    }

    public override void writeWorkerInfo(System.Text.StringBuilder sb) {
        base.writeWorkerInfo(sb);

        sb.Append("Carrying: " + this.plateState.ToString());
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("plateState", (int)this.plateState);
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.plateState = (CellBehaviorTable.EnumPlateState)tag.getInt("plateState");
    }
}
