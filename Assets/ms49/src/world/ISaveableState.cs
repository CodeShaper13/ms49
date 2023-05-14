using fNbt;

/// <summary>
/// Components can implement this to have their state loaded/save
/// when the game is loaded/saved.  For these methods to be invoked,
/// the componet must be on the same object, or on a child object of
/// World.
/// </summary>
public interface ISaveableState {

    string saveableTagName { get; }

    void WriteToNbt(NbtCompound tag);

    void ReadFromNbt(NbtCompound tag);
}
