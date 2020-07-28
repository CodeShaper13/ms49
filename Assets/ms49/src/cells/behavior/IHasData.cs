using fNbt;

/// <summary>
/// Cell behaviors should implement this if they wish to save extra data.
/// </summary>
public interface IHasData {

    void writeToNbt(NbtCompound tag);

    void readFromNbt(NbtCompound tag);
}
