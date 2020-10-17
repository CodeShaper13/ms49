using UnityEngine;

[CreateAssetMenu(fileName = "WorldType", menuName = "MS49/Generation/World Type", order = 1)]
public class WorldType : ScriptableObject {

    [SerializeField]
    private string _mapName = "";
    [SerializeField]
    private GeneratedStructure[] startingStructures = new GeneratedStructure[0];

    public string mapName => this._mapName;
    public GeneratedStructure[] structures => this.startingStructures;
}
