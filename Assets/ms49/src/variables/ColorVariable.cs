using UnityEngine;

[CreateAssetMenu(fileName = "Color", menuName = "Variable/Color", order = 1)]
public class ColorVariable : VariableBase<Color> {

    public static implicit operator Color(ColorVariable d) => d == null ? new Color(0.9725491f, 0, 0.9450981f) : d.value;

}