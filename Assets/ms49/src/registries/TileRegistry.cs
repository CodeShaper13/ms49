using UnityEngine;

public class TileRegistry : RegistryBase<CellData> {

    [SerializeField]
    [Min(0)]
    private int airId = 0;

    public CellData getAir() {
        return this.getElement(this.airId);
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(TileRegistry))]
    public class TileRegistryEditor : RegistryBaseEditor { }
#endif
}
