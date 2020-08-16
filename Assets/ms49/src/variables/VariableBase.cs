using UnityEngine;

public abstract class VariableBase<T> : ScriptableObject, ISerializationCallbackReceiver {

    [SerializeField]
    private T initialValue = default;

    public T value { get; set; }

    public void OnAfterDeserialize() {
        this.value = this.initialValue;
    }

    public void OnBeforeSerialize() { }
}
