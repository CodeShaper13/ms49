using UnityEngine;

public class EntityRegistry : RegistryBase<GameObject> {

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(EntityRegistry))]
    public class EntityRegistryBase : RegistryBaseEditor {

    }
#endif
}