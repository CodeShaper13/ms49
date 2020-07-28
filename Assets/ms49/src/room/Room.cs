using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "MS49/Room", order = 1)]
public class Room : ScriptableObject {

    public string roomName = "nul";
    public string roomDescription;
    public Color regionColor = Color.white;

    public TileGroup[] groups;

    [Serializable]
    public class TileGroup {

        [Min(1)]
        [Tooltip("The minimum number of groups of Cells that this room requires to function")]
        public int minimumGroups = 1;
        public CellData[] cells;
    }   
}
