using fNbt;

public class StatisticInt : StatisticBase<int> {

    public StatisticInt(string displayName, string saveName) : base(displayName, saveName) { }

    public override void increase(int amount = 1) {
        this.value += amount;
    }

    public override void readFromNbt(NbtCompound tag) {
        this.value = tag.GetInt(this.saveName);
    }

    public override void writeToNbt(NbtCompound tag) {
        tag.SetTag(this.saveName, this.value);
    }
}