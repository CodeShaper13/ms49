using UnityEngine;

[CreateAssetMenu(fileName = "Personality", menuName = "MS49/Personality", order = 1)]
public class Personality : ScriptableObject {

    [SerializeField]
    private string _displayName = null;
    [SerializeField, Tooltip("The WorkerTypes that can't have this Personality.")]
    private WorkerType[] _notAllowedTypes = null;
    [SerializeField]
    private EnumPayModifier _payModifier = EnumPayModifier.NORMAL;

    [Space]

    [SerializeField]
    private float _moveSpeedMultiplyer = 1f;
    [SerializeField]
    private float _workSpeedMultiplyer = 1f;

    public string displayName => this._displayName;
    public EnumPayModifier payModifier => this._payModifier;

    public float moveSpeedMultiplyer => this._moveSpeedMultiplyer;
    public float workSpeedMultiplyer => this._workSpeedMultiplyer;

    /// <summary>
    /// Checks if the passed Worker type can have this personality.
    /// </summary>
    public bool canHave(WorkerType type) {
        foreach(WorkerType t in this._notAllowedTypes) {
            if(type == t) {
                return false;
            }
        }

        return true;
    }
}
