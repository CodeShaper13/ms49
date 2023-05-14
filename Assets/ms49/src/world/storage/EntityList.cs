using UnityEngine;
using System.Collections.Generic;
using fNbt;
using System;

public class EntityList : MonoBehaviour, ISaveableState {

    [SerializeField]
    private World _world = null;
    [SerializeField]
    private WorldRenderer _worldRenderer = null;
    [SerializeField]
    private EntityRegistry _entityRegistry = null;

    public List<EntityBase> list { get; private set; }
    /// <summary>
    /// The number of Entities in the world.
    /// </summary>
    public int EntityCount => this.list.Count;

    public string saveableTagName => "entities";

    private void Awake() {
        this.list = new List<EntityBase>();
    }

    private void LateUpdate() {
        // Only show Entities that are at the depth being rendered.
        foreach(EntityBase e in this.list) {
            e.toggleRendererVisability(
                e.depth == this._worldRenderer.getDepthRendering());
        }
    }

    public EntityBase Spawn(NbtCompound tag) {
        int entityId = tag.getInt("id");

        EntityBase entity = this.instantiateObj(entityId);

        if(entity == null) {
            return null;
        }

        entity.initialize(this._world, entityId);
        entity.readFromNbt(tag);

        this.list.Add(entity);

        return entity;
    }

    public EntityBase Spawn(Position position, int entityId) {
        return this.Spawn(
            position.AsVec2 + new Vector2(0.5f, 0.5f), // Center the Entity in the cell.
            position.depth,
            entityId);
    }

    public EntityBase Spawn(Vector2 postion, int depth, int entityId) {
        EntityBase entity = this.instantiateObj(entityId);

        if(entity == null) {
            return null; // Error logged in EntityList#instantiateObj()
        }

        entity.transform.position = postion;
        entity.depth = depth;

        entity.initialize(this._world, entityId);
        entity.onEnterWorld();

        this.list.Add(entity);

        return entity;
    }

    /// <summary>
    /// Removes an Entity from the world.
    /// </summary>
    public void Remove(EntityBase entity) {
        if(entity == null) {
            return;
        }

        this.list.Remove(entity);
        entity.onDestroy();
        GameObject.Destroy(entity.gameObject);
    }

    /// <summary>
    /// Returns the Entity with the passed GUID, or null if there are
    /// no Entities with the passed GUID.
    /// </summary>
    public EntityBase GetEntityFromGuid(Guid guid) {
        foreach(EntityBase e in this.list) {
            if(e.guid == guid) {
                return e;
            }
        }

        return null;
    }

    public void WriteToNbt(NbtCompound tag) {
        NbtList list = new NbtList(NbtTagType.Compound);
        foreach(EntityBase e in this.list) {
            NbtCompound compound = new NbtCompound();
            e.writeToNbt(compound);
            list.Add(compound);
        }
        tag.setTag("entities", list);
    }

    public void ReadFromNbt(NbtCompound tag) {
        NbtList entityTags = tag.getList("entities");
        foreach(NbtCompound tagEntity in entityTags) {
            EntityBase entity = this.Spawn(tagEntity);
        }
    }

    /// <summary>
    /// Instantiates an Entity from a prefab.  If the prefab is missing
    /// a component of type EntityBase, an error is logged.
    /// </summary>
    private EntityBase instantiateObj(int entityId) {
        GameObject prefab = this._entityRegistry[entityId];
        if(prefab != null) {
            EntityBase entity = GameObject.Instantiate(prefab, this.transform).GetComponent<EntityBase>();

            return entity;
        }
        else {
            Debug.LogWarning("Tried to spawn an Entity with an unknown Id ( " + entityId + ")");
            return null;
        }
    }
}
