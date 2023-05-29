using fNbt;
using UnityEngine;

public class EntityItem : EntityBase {

    [SerializeField]
    private SpriteRenderer sr = null;
    [SerializeField]
    private Vector2 spriteRndShift = new Vector2(0.1f, 0.1f);

    public Item item { get; private set; }

    public override void Initialize(World world, int id) {
        base.Initialize(world, id);

        this.sr.transform.localPosition = new Vector2(
            Random.Range(-this.spriteRndShift.x, this.spriteRndShift.x),
            Random.Range(-this.spriteRndShift.y, this.spriteRndShift.y));
    }

    public override void Update() {
        base.Update();

        if(Pause.IsPaused) {
            return;
        }

        if(this.item == null) {
            Debug.LogWarning("EntityItem has no item field set!");
        }
    }

    public override void WriteToNbt(NbtCompound tag) {
        base.WriteToNbt(tag);

        tag.SetTag("itemId", Main.instance.ItemRegistry.GetIdOfElement(this.item));
    }

    public override void ReadFromNbt(NbtCompound tag) {
        base.ReadFromNbt(tag);

        this.SetItem(Main.instance.ItemRegistry[tag.GetInt("itemId")]);
    }

    public void SetItem(Item item) {
        if(item == null) {
            Debug.LogWarning("Can't set EntityItem#item to null");
        } else {
            this.item = item;
            this.sr.sprite = this.item.sprite;
        }
    }
}
