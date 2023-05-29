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
        this.type = Main.instance.WorkerTypeRegistry[tag.GetInt("type")];
        this.endAvailabilityTime = tag.GetDouble("endAvailabilityTime");
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.SetTag("info", this.info.writeToNbt());
        tag.SetTag("type", Main.instance.WorkerTypeRegistry.GetIdOfElement(this.type));
        tag.SetTag("endAvailabilityTime", this.endAvailabilityTime); // TODO should this be a long, will there be an overflow?

        return tag;
    }
}
