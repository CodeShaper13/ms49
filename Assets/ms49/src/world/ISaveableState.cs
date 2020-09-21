using fNbt;

public interface ISaveableSate {

    string tagName { get; }

    void writeToNbt(NbtCompound tag);

    void readFromNbt(NbtCompound tag);
}
