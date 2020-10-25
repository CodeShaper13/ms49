using UnityEngine;
using System.Collections.Generic;
using fNbt;
using System;

public class EntityList : MonoBehaviour, ISaveableState {

    [SerializeField]
    private World world = null;

    public List<EntityBase> list { get; private set; }
    public int count => this.list.Count;

    public string tagName => "entities";

    private WorldRenderer worldRenderer;

    private void Awake() {
        this.list = new List<EntityBase>();
    }

    private void Start() {
        this.worldRenderer = GameObject.FindObjectOfType<WorldRenderer>();
    }

    private void Update() {
        if(!Pause.isPaused()) {
            // Update the Entities.
            for(int i = this.list.Count - 1; i >= 0; i--) {
                this.list[i].onUpdate();
            }
        }
    }

    private void LateUpdate() {
        // Only show Entities that at the depth being rendered.
        foreach(EntityBase e in this.list) {
            e.toggleRendererVisability(
                e.depth == this.worldRenderer.getDepthRendering());
        }

        if(!Pause.isPaused()) {
            // Update the Entities.
            for(int i = this.list.Count - 1; i >= 0; i--) {
                this.list[i].onLateUpdate();
            }
        }
    }

    public EntityBase spawn(NbtCompound tag) {
        int entityId = tag.getInt("id");

        EntityBase entity = this.instantiateObj(entityId);

        if(entity == null) {
            return null; // Error logged in EntityList#instantiateObj()
        }

        entity.initialize(this.world, entityId);

        entity.readFromNbt(tag);

        this.list.Add(entity);

        return entity;
    }

    public EntityBase spawn(Position postion, int entityId) {
        return this.spawn(postion.vec2 + new Vector2(0.5f, 0.5f), postion.depth, entityId);
    }

    public EntityBase spawn(Vector2 postion, int depth, int entityId) {
        EntityBase entity = this.instantiateObj(entityId);

        if(entity == null) {
            return null; // Error logged in EntityList#instantiateObj()
        }

        entity.transform.position = postion;
        entity.depth = depth;

        entity.initialize(this.world, entityId);
        entity.onEnterWorld();

        this.list.Add(entity);

        return entity;
    }

    private EntityBase instantiateObj(int entityId) {
        GameObject prefab = Main.instance.entityRegistry.getElement(entityId);
        if(prefab != null) {
            EntityBase entity = GameObject.Instantiate(prefab, this.transform).GetComponent<EntityBase>();

            return entity;
        } else {
            Debug.LogWarning("Tried to spawn an Entity with an unknown Id ( " + entityId + ")");
            return null;
        }
    }

    public void remove(EntityBase entity) {
        this.list.Remove(entity);
        entity.onDestroy();
        GameObject.Destroy(entity.gameObject);
    }

    /// <summary>
    /// Returns the Entity with the passed GUID, or null if there are
    /// no Entities with the passed GUID.
    /// </summary>
    public EntityBase getEntityFromGuid(Guid guid) {
        foreach(EntityBase e in this.list) {
            if(e.guid == guid) {
                return e;
            }
        }

        return null;
    }

    public void writeToNbt(NbtCompound tag) {
        NbtList list = new NbtList(NbtTagType.Compound);
        foreach(EntityBase e in this.list) {
            NbtCompound compound = new NbtCompound();
            e.writeToNbt(compound);
            list.Add(compound);
        }
        tag.setTag("entities", list);
    }

    public void readFromNbt(NbtCompound tag) {
        NbtList entityTags = tag.getList("entities");
        foreach(NbtCompound tagEntity in entityTags) {
            EntityBase entity = this.spawn(tagEntity);
        }
    }
}
