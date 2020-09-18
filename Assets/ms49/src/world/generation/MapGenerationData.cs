using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationData", menuName = "MS49/Generation/Main", order = 1)]
public class MapGenerationData : ScriptableObject {

    [SerializeField, Min(0)]
    private int _playerStartLayer = 0;
    [SerializeField]
    private LayerDataBase[] layerGenerators = null;

    public StartingStructure[] startingStructures;

    public int layerCount {
        get {
            return this.layerGenerators == null ? 0 : this.layerGenerators.Length;
        }
    }
    public int playerStartLayer { get { return this._playerStartLayer; } }

    /// <summary>
    /// If there is no LayerData for the passed layer, null is returned.
    /// </summary>
    /// <param name="depth"></param>
    /// <returns></returns>
    public LayerDataBase getLayerFromDepth(int depth) {
        if(depth < 0 || depth >= this.layerGenerators.Length) {
            return null;
        }

        return this.layerGenerators[depth];
    }

    [Serializable]
    public class StartingStructure {

        public Position pos;
        public StructureBase structure;
    }
}
