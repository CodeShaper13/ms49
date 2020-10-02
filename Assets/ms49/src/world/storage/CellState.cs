public class CellState {

    public readonly CellData data;
    public readonly CellBehavior behavior;
    public Rotation rotation;

    public CellState(CellData data, CellBehavior behavior, Rotation rotation) {
        this.data = data;
        this.behavior = behavior;
        this.rotation = rotation;
    }

    /// <summary>
    /// Returns true if the state has a behavior.
    /// </summary>
    public bool hasBehavior() {
        return this.behavior != null;
    }
}
