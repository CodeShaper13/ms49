using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationData", menuName = "MS49/Generation/Main", order = 1)]
public class MapGenerationData : ScriptableObject {

    public int mapSize = 128;

    public UnbreakableRockTiles unbreakableRockTiles;
    public TileReferences tiles;
    [SerializeField]
    private LayerDataBase[] layerGenerators = null;

    public StartingStructure[] startingStructures;

    public int layerCount {
        get {
            return this.layerGenerators == null ? 0 : this.layerGenerators.Length;
        }
    }

    public LayerDataBase getLayerFromDepth(int depth) {
        if(depth < 0 || depth >= this.layerGenerators.Length) {
            return null;
        }

        return this.layerGenerators[depth];
    }

    [Serializable]
    public class TileReferences {

        public CellData waterTile = null;
        public CellData lavaTile = null;
        public CellData amethystTile = null;
        public CellData coalTile = null;
        public CellData emeraldTile = null;
        public CellData rubyTile = null;
        public CellData sapphireTile = null;
        public CellData goldTile = null;
    }

    [Serializable]
    public class StartingStructure {

        public Position pos;
        public Structure structure;
    }
}
