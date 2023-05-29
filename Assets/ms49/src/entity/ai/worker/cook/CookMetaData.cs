using fNbt;
using UnityEngine;
using static CellBehaviorTable;

public class CookMetaData : MonoBehaviour, IAiMeta {

    public EnumPlateState plateState { get; set; } = EnumPlateState.CLEAN;

    public void readFromNbt(NbtCompound tag) {
        this.plateState = (EnumPlateState)tag.GetInt("plateState");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.SetTag("plateState", (int)this.plateState);
    }
}
