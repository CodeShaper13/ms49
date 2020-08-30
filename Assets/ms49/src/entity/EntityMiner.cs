using System.Text;
using fNbt;

public class EntityMiner : EntityWorker {

    /// <summary> The item the miner is carrying.  Null if they don't have anything. </summary>
    public Item heldItem {
        get; set;
    }

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);
    }

    public override void writeWorkerInfo(StringBuilder sb) {
        base.writeWorkerInfo(sb);

        sb.Append("Carrying: " + (this.heldItem == null ? "Nothing" : this.heldItem.itemName));
    }

    public override void writeToNbt(NbtCompound tag) {
        base.writeToNbt(tag);

        tag.setTag("heldItemId", Main.instance.itemRegistry.getIdOfElement(this.heldItem)); // Null results in -1 being returned.
    }

    public override void readFromNbt(NbtCompound tag) {
        base.readFromNbt(tag);

        this.heldItem = Main.instance.itemRegistry.getElement(tag.getInt("heldItemId", -1)); // -1 results in null being returned
    }
}
