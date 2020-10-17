using UnityEngine;

[CreateAssetMenu(fileName = "Tab", menuName = "MS49/Buildable/Tab", order = 1)]
public class Tab : ScriptableObject {

    [SerializeField]
    private Sprite _icon = null;
    [SerializeField]
    private string _tabName = "???";
    [SerializeField]
    private bool _onlyInCreative = false;

    public Sprite icon => this._icon;
    public string tabName => this._tabName;
    public bool onlyInCreative => this._onlyInCreative;
}
