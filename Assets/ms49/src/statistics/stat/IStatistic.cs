using fNbt;

public interface IStatistic {

    string saveName { get; }

    string displayName { get; }

    string displayValue { get; }

    void readFromNbt(NbtCompound tag);

    void writeToNbt(NbtCompound tag);
}
