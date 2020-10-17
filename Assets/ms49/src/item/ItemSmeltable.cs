using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "MS49/Item Smeltable", order = 2)]
public class ItemSmeltable : Item {

    [SerializeField]
    private Item _smeltingResult = null;

    public Item smeltingResult => this._smeltingResult;
}
