using fNbt;

public class Candidate {

    public readonly WorkerInfo info;
    public readonly WorkerType type;
    public readonly double endAvailabilityTime;

    public Candidate(WorkerInfo info, WorkerType type, double f) {
        this.info = info;
        this.type = type;
        this.endAvailabilityTime = f;
    }

    public Candidate(NbtCompound tag) {
        this.info = new WorkerInfo(tag.getCompound("info"));
        this.type = Main.instance.WorkerTypeRegistry.GetElement(tag.getInt("type"));
        this.endAvailabilityTime = tag.getDouble("endAvailabilityTime");
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.setTag("info", this.info.writeToNbt());
        tag.setTag("type", Main.instance.WorkerTypeRegistry.GetIdOfElement(this.type));
        tag.setTag("endAvailabilityTime", this.endAvailabilityTime); // TODO should this be a long, will there be an overflow?

        return tag;
    }
}
