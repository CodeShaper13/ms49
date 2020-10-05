using UnityEngine;

public abstract class VariableBase<T> : ScriptableObject, ISerializationCallbackReceiver {

    [SerializeField, TextArea(1, 3)]
    private string docString = "";
    [SerializeField]
    private T initialValue = default;

    public T value { get; set; }

    public void OnAfterDeserialize() {
        this.value = this.initialValue;
    }

    public void OnBeforeSerialize() { }
}
