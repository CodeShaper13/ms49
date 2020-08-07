﻿using UnityEngine;

public class CellBehavior : MonoBehaviour {

    [SerializeField]
    private bool _cache = true;

    public World world {
        get;
        private set;
    }

    public Position pos {
        get;
        private set;
    }

    public bool cache => this._cache;
    public CellData data => this.state.data;
    public Rotation rotation => this.state.rotation;

    private CellState state;

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

    public virtual void onUpdate() { }

    /// <summary>
    /// Called when the Cell is right clicked.
    /// </summary>
    public virtual void onRightClick() { }

    /// <summary>
    /// Called when the Tile is destroyed.  This is where all cleanup should happen, not OnDestroy()
    /// </summary>
    public virtual void onDestroy() { }

    public virtual void onNeighborChange(CellState triggererCell, Position triggererPos) {

    }

    /// <summary>
    /// Dirties the Cell.  This causes it to be redrawn at the end of the frame.
    /// </summary>
    public void dirty() {
        WorldRenderer.instance.dirtyTile(this.pos.vec2Int);
    }
}
