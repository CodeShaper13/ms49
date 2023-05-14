using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour {

    [SerializeField]
    private LayerData[] _layers = new LayerData[0];
    [SerializeField]
    private NoiseSettings _noiseSettings = new NoiseSettings();

    [Space, Header("Player Starting Settings")]

    [SerializeField, Min(0)]
    private int _startingLayer = 0;
    [SerializeField]
    private int _startingMoney = 1000;
    [SerializeField]
    private WorkerType[] _startingWorkers = new WorkerType[0];

    private List<FeatureBase> features;

    public int layerCount => this._layers == null ? 0 : this._layers.Length;
    public int playerStartLayer => this._startingLayer;
    public int startingMoney => this._startingMoney;
    public WorkerType[] startingWorkers => this._startingWorkers;

    private void Awake() {
        // Find all of the attached Feature components and sort them by priority.
        this.features = new List<FeatureBase>();
        foreach(FeatureBase task in this.GetComponentsInChildren<FeatureBase>()) {
            this.features.Add(task);
        }
        this.features = this.features.OrderBy(e => e.priority).ToList();
    }

    /// <summary>
    /// Fully generates the Layer at the passed depth and places it
    /// into the World's storage.
    /// </summary>
    public void generateLayer(World world, int depth) {
        int layerSeed = world.seed * (depth + 1);

        MapAccessor accessor = new MapAccessor(world.MapSize, depth);
        LayerData layerData = this.getLayerFromDepth(depth);


        // Fill the map with the Layer's fill cell.
        for(int x = 0; x < accessor.size; x++) {
            for(int y = 0; y < accessor.size; y++) {
                accessor.setCell(x, y, layerData.getFillCell(world, x, y));
            }
        }


        // Generate all of the features.
        foreach(FeatureBase feature in this.features) {
            feature.generate(new System.Random(layerSeed), layerData, accessor);
        }        


        Layer layer = new Layer(world, depth);


        // Apply the accessor to the Layer.
        for(int x = 0; x < accessor.size; x++) {
            for(int y = 0; y < accessor.size; y++) {
                Rotation r = accessor.getRot(x, y);
                layer.setCell(
                    x,
                    y,
                    accessor.getCell(x, y),
                    r == null ? Rotation.UP.id : r.id,
                    false);
            }
        }


        // Generate the hardness map and apply it.
        float[,] noise = NoisemapGenerator.computeNoiseMap(
            this._noiseSettings,
            world.seed,
            world.MapSize,
            depth);
        for(int x = 0; x < world.MapSize; x++) {
            for(int y = 0; y < world.MapSize; y++) {
                float n = noise[x, y];
                int hardness = n < 0.333f ? 0 : (n < 0.666f ? 1 : 2);
                layer.SetHardness(x, y, hardness);
            }
        }


        world.storage.SetLayer(layer, depth);


        // Generate all of the structures that belong on this Layer
        Random.InitState(layerSeed);
        
        foreach(StructureBase structure in layerData.structures) {
            if(structure != null) {
                structure.generate(world, depth);
            }
        }
    }

    /// <summary>
    /// Returns the LayerData that provides data for the passed depth.
    /// If there is none set or the passed depth is out of bounds, null is returned.
    /// </summary>
    public LayerData getLayerFromDepth(int depth) {
        if(depth < 0 || depth >= this._layers.Length) {
            return null;
        }

        return this._layers[depth];
    }
}
