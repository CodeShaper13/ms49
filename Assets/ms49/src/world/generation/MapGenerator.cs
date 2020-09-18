using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour {

    [SerializeField]
    private MapGenerationData genData = null;

    private List<FeatureBase> features;

    private void Awake() {
        // Find all of the attached Feature components and sort them by priority.
        this.features = new List<FeatureBase>();
        foreach(FeatureBase task in this.GetComponentsInChildren<FeatureBase>()) {
            this.features.Add(task);
        }
        this.features = this.features.OrderBy(e => e.priority).ToList();
    }

    public void generateLayer(World world, int depth) {
        int layerSeed = world.seed * depth;

        MapAccessor accessor = new MapAccessor(world.mapSize);
        LayerDataBase layerData = this.genData.getLayerFromDepth(depth);

        // Fill the map with the Layer's fill cell.
        for(int x = 0; x < accessor.size; x++) {
            for(int y = 0; y < accessor.size; y++) {
                accessor.setCell(x, y, layerData.getFillCell(x, y));
            }
        }

        // Generate all of the features.
        foreach(FeatureBase feature in this.features) {
            feature.generate(new System.Random(layerSeed), layerData, accessor);
        }        

        Layer layer = new Layer(world, depth);
        world.storage.setLayer(layer, depth);

        // Apply the accessor to the Layer.
        for(int x = 0; x < accessor.size; x++) {
            for(int y = 0; y < accessor.size; y++) {
                world.setCell(x, y, depth, accessor.getCell(x, y), false);
            }
        }
    }

    public void generateStartRoom(World world) {
        foreach(MapGenerationData.StartingStructure ss in this.genData.startingStructures) {
            ss.structure.placeIntoWorld(world, ss.pos);
        }
    }
}
