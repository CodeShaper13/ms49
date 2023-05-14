using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "MS49/Item/Item Smeltable", order = 2)]
public class ItemSmeltable : Item {

    [SerializeField, Min(0)]
    private float _smeltingTime = 1f;
    [SerializeField]
    private Item _smeltingResult = null;

    public float SmeltingTime => this._smeltingTime;
    public Item SmeltingResult => this._smeltingResult;
}
