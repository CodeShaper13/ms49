using fNbt;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EntityItem : EntityBase {

    private SpriteRenderer sr;

    public Item item { get; private set; }

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.sr = this.GetComponent<SpriteRenderer>();
    }

    public override void onUpdate() {
        base.onUpdate();
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("itemId", Main.instance.itemRegistry.getIdOfElement(this.item));
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.setItem(Main.instance.itemRegistry.getElement(tag.getInt("itemId")));
    }

    public void setItem(Item item) {
        this.item = item;
        this.sr.sprite = item == null ? null : item.sprite;
    }
}
