using UnityEngine;

[CreateAssetMenu(fileName = "Milestone", menuName = "MS49/Milestone", order = 1)]
public class MilestoneData : ScriptableObject {

    [SerializeField]
    private string _milestoneName = null;
    [SerializeField]
    private MilestoneRequirerment[] _requirements = null;

    public string milestoneName { get { return this._milestoneName; } }
    public MilestoneRequirerment[] requirements { get { return this._requirements; } }
}
