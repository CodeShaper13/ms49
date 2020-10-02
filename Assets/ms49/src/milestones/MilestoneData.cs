using UnityEngine;

[CreateAssetMenu(fileName = "Milestone", menuName = "MS49/Milestone/Milestone", order = 1)]
public class MilestoneData : ScriptableObject, ISerializationCallbackReceiver {

    [SerializeField]
    private string _milestoneName = null;
    [SerializeField]
    private MilestoneRequirerment[] _requirements = null;
    [SerializeField, Tooltip("If true, a layer is unlocked when completing this milestone")]
    private bool _unlocksLayer = false;
    [SerializeField]
    private BuildableBase[] _unlockedBuildables = null;
    [SerializeField]
    private WorkerType[] _unlockedWorkerTypes = null;
    [SerializeField]
    private int _hireCandaditeCount = 0;

    public string milestoneName { get { return this._milestoneName; } }
    public MilestoneRequirerment[] requirements { get { return this._requirements; } }
    public bool unlocksLayer { get { return this._unlocksLayer; } }
    public bool isUnlocked { get; set; }
    public BuildableBase[] unlockedBuildables { get { return this._unlockedBuildables; } }
    public WorkerType[] unlockedWorkerTypes => this._unlockedWorkerTypes;
    public int hireCandaditeCount => this._hireCandaditeCount;

    public void OnAfterDeserialize() {
        this.isUnlocked = false;
    }

    public void OnBeforeSerialize() { }

    /// <summary>
    /// Returns true if all of the requirements for this milestone have
    /// been met.
    /// </summary>
    public bool allRequiermentMet(World world) {
        if(this.requirements == null) {
            return false;
        }

        foreach(MilestoneRequirerment r in this.requirements) {
            if(r == null) {
                continue;
            }

            if(!r.isMet(world)) {
                return false;
            }
        }

        return true;
    }
}
