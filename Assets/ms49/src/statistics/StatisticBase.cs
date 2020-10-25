using fNbt;

public abstract class StatisticBase<T> : IStatistic {

    protected T value;

    public string saveName { get; }

    public string displayName { get; }

    public string displayValue => this.value.ToString();

    public StatisticBase(string displayName, string saveName) {
        this.displayName = displayName;
        this.saveName = "stat." + saveName;
    }

    public T get() {
        return this.value;
    }

    public abstract void increase(T amount);

    public abstract void readFromNbt(NbtCompound tag);

    public abstract void writeToNbt(NbtCompound tag);

    public override string ToString() {
        return this.displayName + ": " + this.value.ToString();
    }
}