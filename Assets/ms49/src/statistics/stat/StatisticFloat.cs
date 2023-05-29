using fNbt;

public class StatisticFloat : StatisticBase<float> {

    public StatisticFloat(string displayName, string saveName) : base(displayName, saveName) { }

    public override void increase(float amount) {
        this.value += amount;
    }

    public override void readFromNbt(NbtCompound tag) {
        this.value = tag.GetFloat(this.saveName);
    }

    public override void writeToNbt(NbtCompound tag) {
        tag.SetTag(this.saveName, this.value);
    }
}