﻿using UnityEngine;
using fNbt;

public class CellBehaviorContainer : CellBehavior, IHasData, IContainer {

    [SerializeField]
    private Inventory _inventory = null;

    protected Inventory inventory => this._inventory;
    public virtual bool IsFull => this.inventory.IsFull;
    public virtual bool IsEmpty => this.inventory.IsEmpty;

    public override void OnCreate(World world, CellState state, Position pos) {
        base.OnCreate(world, state, pos);
    }

    public override void onRightClick() {
        base.onRightClick();

        PopupContainer popup = Main.instance.findPopup<PopupContainer>();
        if(popup != null) {
            popup.open();
            popup.SetInventory(this.inventory);
        } else {
            Debug.LogWarning("Could not find PopupContainer");
        }
    }

    public override string getTooltipText() {
        return "[rmb] open container";
    }

    public override void onDestroy() {
        base.onDestroy();

        for(int i = 0; i < this.inventory.GetItemCount(); i++) {
            Item item = this.inventory[i];
            if(item != null) {
                float f = 0.4f;
                Vector2 v = new Vector2(
                    this.pos.x + 0.5f + Random.Range(-f, f),
                    this.pos.y + 0.1f + Random.Range(-f, f));
                EntityItem e = (EntityItem) this.world.entities.Spawn(
                    v,
                    this.pos.depth,
                    3);
                e.setItem(item);
            }
        }
    }

    public virtual void WriteToNbt(NbtCompound tag) {
        if(this.inventory != null) {
            tag.setTag("inventory", this.inventory.WriteToNbt());
        }
    }

    public virtual void ReadFromNbt(NbtCompound tag) {
        if(this.inventory != null) {
            this.inventory.ReadFromNbt(tag.getCompound("inventory"));
        }
    }

    public virtual bool Deposit(Item item) {
        return this.inventory.AddItem(item);
    }

    public virtual Item PullItem() {
        return this.inventory.PullItem();
    }
}
