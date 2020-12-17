using UnityEngine;
using System;

[Serializable]
public class LootTableEntry {

    public Item item = null;
    [Min(1)]
    public int weight = 1;
    [Tooltip("Disabled entries are ignored.")]
    public bool disabled = false;
}
