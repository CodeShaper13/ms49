using UnityEngine;

[CreateAssetMenu(fileName = "Integer", menuName = "Variable/Integer", order = 1)]
public class IntVariable : VariableBase<int> {

    public static implicit operator int(IntVariable i) => i.value;
}