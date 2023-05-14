using UnityEngine;

[CreateAssetMenu(fileName = "Integer", menuName = "Variable/Integer", order = 1)]
public class IntVariable : GenericVariable<int> {

    public static implicit operator int(IntVariable i) => i.value;
}