using UnityEngine;
using System.Collections;

public class WorkerTypeRegistry : RegistryBase<WorkerType> {

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(WorkerTypeRegistry))]
    public class WorkerTypeRegistryEditor : RegistryBaseEditor {
    }
#endif
}
