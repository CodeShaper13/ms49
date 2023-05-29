using UnityEngine;

public abstract class StructureBase : MonoBehaviour, IPriority {

    [SerializeField]
    private int _priority = 0;

    public int Priority => this._priority;

    // Makes the enable checkbox show up in the inspector.
    private void Start() { }

    /// <summary>
    /// Called to generate the structure after all features have been generated.
    /// </summary>
    public abstract void GenerateStructure(World world, int depth);
}