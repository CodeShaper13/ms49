using UnityEngine;
using fNbt;

public abstract class EntityBase : MonoBehaviour {

    public World world { get; private set; }
    public int depth { get; set; }
    public int id { get; private set; }

    private void Awake() { } // Stop child classes from overriding.

    private void Start() { } // Stop child classes from overriding.

    public virtual void initialize(World world, int id, int depth) {
        this.world = world;
        this.id = id;
        this.depth = depth;
    }

    public virtual void onUpdate() { }

    public virtual void onDestroy() { }

    /// <summary>
    /// Gets the Entity's position in cell units.
    /// </summary>
    public Vector2Int getCellPos() {
        return this.world.worldToCell(this.transform.position);
    }

    /// <summary>
    /// Get's the Entity's position in cell units.
    /// </summary>
    public Position position {
        get { return new Position(this.getCellPos(), this.depth); }
    }

    /// <summary>
    /// Get's the Entity's position in world units.
    /// </summary>
    public Vector2 worldPos {
        get {
            return this.transform.position;
        }
        set {
            this.transform.position = value;
        }
    }

    public virtual void writeToNbt(NbtCompound tag) {
        tag.setTag("id", this.id);
        tag.setTag("position", this.worldPos);
        tag.setTag("depth", this.depth);
    }

    public virtual void readFromNbt(NbtCompound tag) { }
}
