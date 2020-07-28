using System;
using UnityEngine;

[Serializable]
public class OreSettings {

    public OreType type;
    public int veinCount = 1;
    [MinMaxSlider(1, 12)]
    public Vector2Int veinSize = new Vector2Int(1, 1);

}
