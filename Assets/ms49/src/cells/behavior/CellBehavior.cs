using System.Collections.Generic;
using UnityEngine;

public abstract class CellBehavior : MonoBehaviour {

    [SerializeField]
    private bool _cache = true;

    protected CellState state;

    public World world { get; private set; }
    public Position pos { get; private set; }

    public bool cache => this._cache;
    public CellData data => this.state.data;
    public Rotation rotation => this.state.rotation;
    /// <summary> The center of the Behavior's cell in world units. </summary>
    public Vector2 center => this.pos.vec2 + new Vector2(0.5f, 0.5f);

    private void Update() {
        if(!Pause.isPaused()) {
            this.onUpdate();
        }
    }

    public virtual void onCreate(World world, CellState state, Position pos) {
        this.world = world;
        this.state = state;
        this.pos = pos;
    }

    /// <summary>
    /// Called every frame while the game is not paused.
    /// </summary>
    public virtual void onUpdate() { }

    /// <summary>
    /// Called when the Cell is right clicked.
    /// </summary>
    public virtual void onRightClick() { }

    /// <summary>
    /// Called when the Tile is destroyed.  This is where all cleanup should happen, not OnDestroy()
    /// </summary>
    public virtual void onDestroy() { }

    public virtual void onNeighborChange(CellState triggererCell, Position triggererPos) { }

    /// <summary>
    /// Dirties the Cell.  This causes it to be redrawn at the end of the frame.
    /// </summary>
    public void dirty() {
        this.world.worldRenderer.dirtyTile(this.pos.x, this.pos.y);
    }

    public virtual string getTooltipText() {
        return null;
    }

    public virtual void getDebugText(List<string> s) {
        s.Add("Type: " + this.GetType().ToString());
    }
}
