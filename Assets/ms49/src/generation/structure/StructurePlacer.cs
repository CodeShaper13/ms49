using UnityEngine;

public class StructurePlacer : MonoBehaviour, IPriority {

    [SerializeField]
    private int _priority = 0;
    [SerializeField]
    private StructureBase _structure = null;

    public int Priority => this._priority;

    // Makes the enable checkbox show up in the inspector.
    private void Start() { }

    /// <summary>
    /// Called to generate the structure after all features have been generated.
    /// </summary>
    public void GenerateStructure(World world, int depth) {
        if(this._structure != null) {
            this._structure.Generate(world, depth);
        }
    }
}