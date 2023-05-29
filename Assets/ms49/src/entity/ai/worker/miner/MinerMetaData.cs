using UnityEngine;
using fNbt;

public class MinerMetaData : MonoBehaviour, IAiMeta {

    /// <summary> The item the miner is carrying.  Null if they don't have anything. </summary>
    public Item heldItem { get; set; }

    public void readFromNbt(NbtCompound tag) {
        int itemId = tag.GetInt("heldItemId", -1);
        if(itemId != -1) {
            this.heldItem = Main.instance.ItemRegistry[itemId];
        }
    }

    public void writeToNbt(NbtCompound tag) {
        if(this.heldItem != null) {
            tag.SetTag("heldItemId", Main.instance.ItemRegistry.GetIdOfElement(this.heldItem));
        }
    }
}
