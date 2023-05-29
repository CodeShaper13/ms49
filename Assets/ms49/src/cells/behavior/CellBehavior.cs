using System.Text;
using UnityEngine;

public abstract class CellBehavior : MonoBehaviour, ITooltipPrompt {

    [SerializeField, Tooltip("If true, this behavior is added to a speedy lookup list")]
    private bool _cache = true;

    protected CellState state;

    public World world { get; private set; }
    public Position pos { get; private set; }

    public bool cache => this._cache;
    public CellData data => this.state.data;
    public Rotation rotation => this.state.Rotation;
    /// <summary> The center of the Behavior's cell in world units. </summary>
    public Vector2 center => this.pos.AsVec2 + new Vector2(0.5f, 0.5f);

    // ITooltipPrompt implementation.
    public string Text => this.GetTooltipText();
    public bool OverrideDelay => false;
    public float Delay => 0f;

    public virtual void OnCreate(World world, CellState state, Position pos) {
        this.world = world;
        this.state = state;
        this.pos = pos;
    }

    /// <summary>
    /// Called when the Cell is right clicked.
    /// </summary>
    public virtual void OnRightClick() { }

    /// <summary>
    /// Called when the Tile is destroyed.  This is where all cleanup should happen, not OnDestroy()
    /// </summary>
    public virtual void OnBehaviorDestroy() { }

    public virtual void onNeighborChange(CellState triggererCell, Position triggererPos) { }

    /// <summary>
    /// Dirties the Cell.  This causes it to be redrawn at the end of the frame.
    /// </summary>
    public void dirty() {
        this.world.worldRenderer.dirtyTile(this.pos.x, this.pos.y);
    }

    public virtual string GetTooltipText() {
        return null;
    }

    public virtual void GetDebugText(StringBuilder sb, string indent) {
        sb.AppendLine(indent + "Type: " + this.GetType().ToString());
    }
}
