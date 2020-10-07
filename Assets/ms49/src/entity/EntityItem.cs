using fNbt;
using UnityEngine;

public class EntityItem : EntityBase {

    [SerializeField]
    private SpriteRenderer sr = null;
    [SerializeField]
    private Vector2 spriteRndShift = new Vector2(0.1f, 0.1f);

    public Item item { get; private set; }

    public override void initialize(World world, int id) {
        base.initialize(world, id);

        this.sr.transform.localPosition = new Vector2(
            Random.Range(-this.spriteRndShift.x, this.spriteRndShift.x),
            Random.Range(-this.spriteRndShift.y, this.spriteRndShift.y));
    }

    public override void onUpdate() {
        base.onUpdate();

        if(this.item == null) {
            Debug.LogWarning("EntityItem has no item field set!");
        }
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
        if(item == null) {
            Debug.LogWarning("Can't set EntityItem#item to null");
        } else {
            this.item = item;
            this.sr.sprite = this.item.sprite;
        }
    }
}
