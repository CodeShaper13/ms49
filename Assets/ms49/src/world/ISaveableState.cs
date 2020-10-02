using fNbt;

public interface ISaveableState {

    string tagName { get; }

    void writeToNbt(NbtCompound tag);

    void readFromNbt(NbtCompound tag);
}
