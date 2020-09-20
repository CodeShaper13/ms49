using fNbt;

public interface IAiMeta {

    void writeToNbt(NbtCompound tag);

    void readFromNbt(NbtCompound tag);
}
