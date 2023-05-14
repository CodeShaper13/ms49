using fNbt;

public class Trainee {

    public readonly WorkerInfo info;
    public readonly WorkerType type;

    public Trainee(WorkerInfo info, WorkerType type) {
        this.info = info;
        this.type = type;
    }

    public Trainee(NbtCompound tag) {
        this.info = new WorkerInfo(tag.getCompound("info"));
        this.type = Main.instance.WorkerTypeRegistry.GetElement(tag.getInt("type"));
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.setTag("info", this.info.writeToNbt());
        tag.setTag("type", Main.instance.WorkerTypeRegistry.GetIdOfElement(this.type));

        return tag;
    }
}
