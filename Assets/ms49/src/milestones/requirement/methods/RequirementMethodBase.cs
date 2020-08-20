using UnityEngine;

public abstract class RequirementMethodBase : ScriptableObject {

    [SerializeField]
    private string _requirementTitle = null;
    [SerializeField, Tooltip("If empty, no tooltip will be shows. {0} maps to requirements target amount")]
    private string _helpTooltip = null;

    public string requirementTitle {
        get {
            return this._requirementTitle;
        }
    }

    public string helpTooltip {
        get {
            return this._helpTooltip;
        }
    }

    /// <summary>
    /// Returns the progress towards this requirement.
    /// </summary>
    public abstract int getProgress(World world);
}
