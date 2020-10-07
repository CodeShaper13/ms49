﻿using UnityEngine;
using fNbt;
using System;

public abstract class EntityBase : MonoBehaviour {

    [SerializeField]
    private GameObject[] renderers = new GameObject[0];

    private bool areRenderersVisible = false;

    public World world { get; private set; }
    public int depth { get; set; }
    public int id { get; private set; }
    public Guid guid { get; private set; }

    private void Awake() { } // Stop child classes from overriding.

    private void Start() { } // Stop child classes from overriding.

    public virtual void initialize(World world, int id) {
        this.world = world;
        this.id = id;
        this.depth = depth;
    }

    /// <summary>
    /// Called when the Entity enter the World for the first time.
    /// </summary>
    public virtual void onEnterWorld() {
        this.guid = Guid.NewGuid();
    }

    public virtual void onRenderingEnable() { }

    public virtual void onRenderingDisable() { }

    /// <summary>
    /// Called every frame the game is not paused.
    /// </summary>
    public virtual void onUpdate() { }

    public virtual void onDestroy() { }

    public void toggleRendererVisability(bool visible) {
        if(this.areRenderersVisible == visible) {
            return; // Nothing changes
        } else {
            // Enable/disable colliders
            foreach(Collider2D sr in this.GetComponentsInChildren<Collider2D>()) {
                sr.enabled = visible;
            }

            foreach(GameObject obj in this.renderers) {
                if(obj != null) {
                    obj.SetActive(visible);
                }
            }

            if(visible) {
                this.onRenderingEnable();
            } else {
                this.onRenderingDisable();
            }
        }
    }

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
        get {
            return new Position(this.getCellPos(), this.depth);
        }
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
        tag.setTag("guid", this.guid);
    }

    public virtual void readFromNbt(NbtCompound tag) {
        this.transform.position = tag.getVector2("position");
        this.depth = tag.getInt("depth");
        this.guid = tag.getGuid("guid");
    }
}
