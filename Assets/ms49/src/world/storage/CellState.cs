using UnityEngine;

public class CellState {

    public readonly CellData data;
    /// <summary>
    /// The Cell's behavior.  Null if the Cell has no associated Behavior.
    /// </summary>
    public readonly CellBehavior behavior;
    public int meta;
    /// <summary>
    /// 
    /// </summary>
    public readonly Vector2Int parent;

    /// <summary>
    /// The Cell's meta as a rotation.
    /// </summary>
    public Rotation Rotation {
        get => Rotation.ALL[Mathf.Clamp(this.meta, 0, 3)];
        set {
            this.meta = value.id;
        }
    }
    /// <summary>
    /// True if the state has a behavior.
    /// </summary>
    public bool HasBehavior => this.behavior != null;
    public bool HasParent => this.parent != Vector2Int.zero;


    public CellState(CellData data, CellBehavior behavior, int meta) :
        this(data, behavior, meta, Vector2Int.zero) {
    }

    public CellState(CellData data, CellBehavior behavior, Rotation rotation) :
        this(data, behavior, rotation, Vector2Int.zero) {
    }

    public CellState(CellData data, CellBehavior behavior, int meta, Vector2Int parent) {
        this.data = data;
        this.behavior = behavior;
        this.meta = meta;
        this.parent = parent;
    }

    public CellState(CellData data, CellBehavior behavior, Rotation rotation, Vector2Int parent) :
        this(data, behavior, rotation.id, parent) {
    }
}
