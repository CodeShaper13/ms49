using UnityEngine;

[CreateAssetMenu(fileName = "Registry", menuName = "MS49/Registry/Cells", order = 1)]
public class CellDataRegistry : Registry<CellData> {

    [Space]
    [SerializeField, Min(0)]
    private int _airId = 0;

    public CellData GetAir() {
        return this[this._airId];
    }
}