using UnityEngine;

public abstract class FeatureBase : MonoBehaviour {

    [SerializeField]
    private int _priority = 0;

    public int priority => this._priority;

    public abstract void generate(System.Random rnd, LayerData layerData, MapAccessor accessor);
}
