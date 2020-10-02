using System;
using UnityEngine;

[Serializable]
public class MilestoneRequirerment {

    [SerializeField]
    private RequirementMethodBase _method = null;
    [SerializeField, Min(0)]
    private int _targetAmount = 1;

    public int targetAmount {
        get {
            return this._targetAmount;
        }
    }

    public string title {
        get {
            return this._method != null ? this._method.requirementTitle : string.Empty; 
        }
    }

    public string tooltip {
        get {
            return this._method != null ? this._method.helpTooltip : string.Empty;
        }
    }

    /// <summary>
    /// Returns true if the requirement has been met.
    /// </summary>
    public bool isMet(World world) {
        if(this._method == null) {
            return false;
        } else {
            return this.getProgress(world) >= this._targetAmount;
        }
    }

    /// <summary>
    /// Returns the progress towards this requirement.
    /// </summary>
    public int getProgress(World world) {
        if(this._method == null) {
            return 0;
        } else {
            return this._method.getProgress(world);
        }
    }
}
