using fNbt;

/// <summary>
/// Cell Behaviors should implement this if they wish to save extra data.
/// </summary>
public interface IHasData {

    void WriteToNbt(NbtCompound tag);

    void ReadFromNbt(NbtCompound tag);
}
