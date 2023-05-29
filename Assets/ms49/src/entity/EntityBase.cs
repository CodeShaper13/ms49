using UnityEngine;
using fNbt;
using System;
using System.Text;

public abstract class EntityBase : MonoBehaviour {

    [SerializeField]
    private GameObject[] renderers = new GameObject[0];

    private Collider2D[] colliders;
    private bool areRenderersVisible = true;

    public World world { get; private set; }
    public int depth { get; set; }
    public int id { get; private set; }
    public Guid guid { get; private set; }
    public Rotation rotation { get; set; }

    private void Awake() { } // Stop child classes from overriding.

    private void Start() { // Stop child classes from overriding.
        this.colliders = this.GetComponentsInChildren<Collider2D>();

        this.SetRendererVisability(false);

        this.name += "(" + guid.ToString() + ")";
    }

    public virtual void Initialize(World world, int id) {
        this.world = world;
        this.id = id;
        this.depth = 0;
        this.rotation = Rotation.DOWN;
    }

    /// <summary>
    /// Called when the Entity enter the World for the first time.
    /// </summary>
    public virtual void OnEnterWorld() {
        this.guid = Guid.NewGuid();
    }

    public virtual void OnRenderingEnable() { }

    public virtual void OnRenderingDisable() { }

    public virtual void Update() { }

    public virtual void LateUpdate() { }

    public virtual void onDestroy() { }

    /// <summary>
    /// Called when the Entity is right clicked.
    /// </summary>
    public virtual void OnRightClick() { }

    public virtual void getDebugText(StringBuilder sb, string indent) { }

    /// <summary>
    /// If true if returned, this Entity can be destroyed with the
    /// demo popup.  The Entity must also have a collider so it can
    /// be found.
    /// </summary>
    public virtual bool isDestroyable() {
        return false;
    }

    public void SetRendererVisability(bool visible) {
        if(visible == this.areRenderersVisible) {
            return; // Nothing changes.
        } else {
            // Enable/disable colliders.
            foreach(Collider2D sr in this.colliders) {
                sr.enabled = visible;
            }

            foreach(GameObject obj in this.renderers) {
                if(obj != null) {
                    obj.SetActive(visible);
                }
            }

            if(visible) {
                this.OnRenderingEnable();
            } else {
                this.OnRenderingDisable();
            }

            this.areRenderersVisible = visible;
        }
    }

    /// <summary>
    /// Gets the Entity's position in cell units.
    /// </summary>
    public Vector2Int GetCellPos() {
        return this.world.WorldToCell(this.transform.position);
    }

    /// <summary>
    /// Get's the Entity's position in cell units.
    /// </summary>
    public Position Position => new Position(this.GetCellPos(), this.depth);

    /// <summary>
    /// Get's the Entity's position in world units.
    /// </summary>
    public Vector2 WorldPos {
        get => this.transform.position;
        set {
            this.transform.position = value;
        }
    }

    public virtual void WriteToNbt(NbtCompound tag) {
        tag.SetTag("id", this.id);
        tag.SetTag("position", this.WorldPos);
        tag.SetTag("depth", this.depth);
        tag.SetTag("facing", this.rotation);
        tag.SetTag("guid", this.guid);
    }

    public virtual void ReadFromNbt(NbtCompound tag) {
        // id tag is read elsewhere.
        this.transform.position = tag.GetVector2("position");
        this.depth = tag.GetInt("depth");
        this.rotation = tag.GetRotation("facing");
        this.guid = tag.GetGuid("guid");
    }
}
