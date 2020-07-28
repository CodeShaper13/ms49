using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "MS49/Mined Item", order = 1)]
public class Item : ScriptableObject {

    public Sprite sprite;
    public string itemName = "nul";
    [Min(0), Tooltip("How much the money is worth.")]
    public int moneyValue;
}
