public class MinedItemRegistry : RegistryBase<Item> {

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MinedItemRegistry))]
    public class EntityRegistryBase : RegistryBaseEditor { }
#endif
}
