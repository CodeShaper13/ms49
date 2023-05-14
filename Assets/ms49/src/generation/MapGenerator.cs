using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

public class MapGenerator : MonoBehaviour {

    [SerializeField]
    private LayerData[] _layers = new LayerData[0];
    [SerializeField]
    private Transform[] _layerObjs = null;
    [SerializeField]
    private NoiseSettings _noiseSettings = new NoiseSettings();

    [SerializeField, BoxGroup("Player Starting Settings"), Min(0)]
    private int _startingLayer = 0;
    [SerializeField, BoxGroup("Player Starting Settings")]
    private int _startingMoney = 1000;
    [SerializeField, BoxGroup("Player Starting Settings")]
    private WorkerType[] _startingWorkers = new WorkerType[0];

    //private List<FeatureBase> features;

    public int LayerCount => this._layers == null ? 0 : this._layers.Length;
    public int PlayerStartLayer => this._startingLayer;
    public int StartingMoney => this._startingMoney;
    public WorkerType[] StartingWorkers => this._startingWorkers;

    /*
    private void Awake() {
        // Find all of the attached Feature components and sort them by priority.
        this.features = new List<FeatureBase>();
        foreach(FeatureBase task in this.GetComponentsInChildren<FeatureBase>()) {
            this.features.Add(task);
        }
        this.features = this.features.OrderBy(e => e.Priority).ToList();
    }
    */

    /// <summary>
    /// Fully generates the Layer at the passed depth and places it
    /// into the World's storage.
    /// </summary>
    public void generateLayer(World world, int depth) {
        int layerSeed = world.seed | (depth + 1).GetHashCode(); // TODO is this a good algorithm?

        MapAccessor accessor = new MapAccessor(world.MapSize, depth);
        LayerData layerData = this.GetLayerFromDepth(depth);

        // Fill the map with the Layer's fill cell.
        for(int x = 0; x < accessor.size; x++) {
            for(int y = 0; y < accessor.size; y++) {
                accessor.SetCell(x, y, layerData.GetFillCell(world, x, y));
            }
        }


        // Generate all of the features.
        foreach(FeatureBase feature in this.GetGeneratorComponents<FeatureBase>(depth)) {
            if(feature.enabled) {
                feature.Generate(
                    new System.Random(layerSeed),
                    layerData,
                    accessor);
            }
        }


        Layer layer = new Layer(world, depth);
        accessor.ApplyToLayer(layer);


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


        // Generate all of the structures that belong on this Layer.
        Random.InitState(layerSeed);
        
        foreach(StructurePlacer structure in this.GetGeneratorComponents<StructurePlacer>(depth)) {
            structure.GenerateStructure(world, depth);
        }
    }

    /// <summary>
    /// Returns the LayerData that provides data for the passed depth.
    /// If there is none set or the passed depth is out of bounds, null is returned.
    /// </summary>
    public LayerData GetLayerFromDepth(int depth) {
        if(depth < 0 || depth >= this._layers.Length) {
            return null;
        }

        return this._layers[depth];
    }

    private IOrderedEnumerable<T> GetGeneratorComponents<T>(int depth) where T : Component, IPriority {
        List<T> components = new List<T>();

        foreach(T component in this.GetComponents<T>()) {
            components.Add(component);
        }

        if(depth >= 0 && depth < this._layerObjs.Length) {
            Transform t = this._layerObjs[depth];
            if(t != null) {
                foreach(T component in t.GetComponentsInChildren<T>()) {
                    components.Add(component);
                }
            }
        }

        return components.OrderBy(e => e.Priority);
    }
}
