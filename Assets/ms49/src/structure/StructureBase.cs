using UnityEngine;

public abstract class StructureBase : ScriptableObject {

    public abstract void placeIntoWorld(World world, Position pos);
}
