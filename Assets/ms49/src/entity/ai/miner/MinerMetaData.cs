using UnityEngine;
using fNbt;

public class MinerMetaData : MonoBehaviour, IAiMeta {

    /// <summary> The item the miner is carrying.  Null if they don't have anything. </summary>
    public Item heldItem {
        get; set;
    }

    public void readFromNbt(NbtCompound tag) {
        this.heldItem = Main.instance.itemRegistry.getElement(tag.getInt("heldItemId", -1)); // -1 results in null being returned
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("heldItemId", Main.instance.itemRegistry.getIdOfElement(this.heldItem)); // Null results in -1 being returned.
    }
}
