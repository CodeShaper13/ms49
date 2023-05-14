using UnityEngine;
using Random = System.Random;

public abstract class FeatureBase : MonoBehaviour, IPriority {

    [SerializeField, Tooltip("Lower priority features are generated before higher priority features.")]
    private int _priority = 0;

    public int Priority => this._priority;

    // Makes the enable checkbox show up in the inspector.
    private void Start() { }

    public abstract void Generate(Random rnd, LayerData layerData, MapAccessor accessor);
}
