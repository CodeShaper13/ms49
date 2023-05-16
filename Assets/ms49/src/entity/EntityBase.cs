﻿using UnityEngine;
using fNbt;
using System;
using System.Text;
using System.Collections.Generic;

public abstract class EntityBase : MonoBehaviour {

    [SerializeField]
    private GameObject[] renderers = new GameObject[0];

    private bool areRenderersVisible = true;

    public World world { get; private set; }
    public int depth { get; set; }
    public int id { get; private set; }
    public Guid guid { get; private set; }
    public Rotation rotation { get; set; }

    private void Awake() { } // Stop child classes from overriding.

    private void Start() { // Stop child classes from overriding.
        this.toggleRendererVisability(false);

        this.name += "(" + guid.ToString() + ")";
    }

    public virtual void initialize(World world, int id) {
        this.world = world;
        this.id = id;
        this.depth = depth;
        this.rotation = Rotation.DOWN;
    }

    /// <summary>
    /// Called when the Entity enter the World for the first time.
    /// </summary>
    public virtual void onEnterWorld() {
        this.guid = Guid.NewGuid();
    }

    public virtual void onRenderingEnable() { }

    public virtual void onRenderingDisable() { }

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

    public void toggleRendererVisability(bool visible) {
        if(visible == this.areRenderersVisible) {
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

            this.areRenderersVisible = visible;
        }
    }

    /// <summary>
    /// Gets the Entity's position in cell units.
    /// </summary>
    public Vector2Int getCellPos() {
        return this.world.WorldToCell(this.transform.position);
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
        tag.setTag("facing", this.rotation.id);
        tag.setTag("guid", this.guid);
    }

    public virtual void readFromNbt(NbtCompound tag) {
        this.transform.position = tag.getVector2("position");
        this.depth = tag.getInt("depth");
        this.rotation = Rotation.ALL[Mathf.Clamp(tag.getInt("facing"), 0, 3)];
        this.guid = tag.getGuid("guid");
    }
}
