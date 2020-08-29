using UnityEngine;
using System.Collections.Generic;
using fNbt;

public class EntityList : MonoBehaviour {

    [SerializeField]
    private World world = null;

    public List<EntityBase> list { get; private set; }

    private Transform entityHolder;
    private WorldRenderer worldRenderer;

    private void Awake() {
        this.list = new List<EntityBase>();
        this.entityHolder = this.world.createHolder("ENTITY_HOLDER");
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
            if(e.depth == this.worldRenderer.targetLayer.depth) {
                e.gameObject.SetActive(true);
            }
            else {
                e.gameObject.SetActive(false);
            }
        }
    }

    public EntityBase spawn(Position postion, int entityId) {
        return this.spawn(postion.vec2 + new Vector2(0.5f, 0.5f), postion.depth, entityId);
    }

    public EntityBase spawn(Vector2 postion, int depth, int entityId) {
        GameObject prefab = Main.instance.entityRegistry.getElement(entityId);
        if(prefab != null) {
            EntityBase entity = GameObject.Instantiate(prefab, this.entityHolder).GetComponent<EntityBase>();
            entity.transform.position = postion;
            entity.initialize(this.world, entityId, depth);

            this.list.Add(entity);

            return entity;
        }
        else {
            Debug.LogWarning("Tried to spawn an Entity with an unknown Id ( " + entityId + ")");
            return null;
        }
    }

    public void remove(EntityBase entity) {
        this.list.Remove(entity);
        entity.onDestroy();
        GameObject.Destroy(entity.gameObject);
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
        foreach(NbtCompound t in entityTags) {
            EntityBase entity = this.spawn(
                t.getVector2("position"),
                t.getInt("depth"),
                t.getInt("id"));
            if(entity != null) {
                entity.readFromNbt(t);
            }
        }
    }
}
