using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LootTable", menuName = "MS49/Loot Table", order = 1)]
public class LootTable : ScriptableObject {

    [MinMaxSlider(1, 32)]
    [Tooltip("How many entries are used to generate loot.")]
    public Vector2Int rolls = new Vector2Int(1, 1);
    [SerializeField]
    private LootTableEntry[] entires = null;

    /// <summary>
    /// Returns the contents of the LootTable.  An empty array is
    /// returned if the table is empty.
    /// </summary>
    public List<Item> getRandomItems() {
        List<Item> items = new List<Item>();

        // Compute the total weight of all entries in the table.
        int totalWeight = 0;
        foreach(LootTableEntry entry in this.entires) {
            totalWeight += entry.weight;
        }

        int rolls = Random.Range(this.rolls.x, this.rolls.y);
        for(int i = 0; i < rolls; i++) {
            int resultNum = Random.Range(0, totalWeight);

            int j = 0;
            foreach(LootTableEntry entry in this.entires) {
                j += entry.weight;
                if(resultNum <= j) {
                    items.Add(entry.item);
                    break;
                }
            }
        }

        return items;
    }
}

