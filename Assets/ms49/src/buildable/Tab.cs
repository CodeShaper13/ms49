using UnityEngine;

[CreateAssetMenu(fileName = "Tab", menuName = "MS49/Buildable/Tab", order = 1)]
public class Tab : ScriptableObject {

    [SerializeField, Tooltip("This tab, and it's contents, will only show up in creative mode.")]
    private bool _onlyInCreative = false;

    public bool onlyInCreative => this._onlyInCreative;
}
