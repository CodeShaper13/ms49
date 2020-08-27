using UnityEngine;

/// <summary>
/// Enables or Disables a GameObject based on the unlock statis of a
/// Milestone.
/// </summary>
public class EnableWithMilestone : MonoBehaviour {

    [SerializeField]
    private GameObject targetObject = null;
    [SerializeField]
    private MilestoneData milestone = null;
    [SerializeField]
    private bool invertResult = false;
    [SerializeField]
    private bool enableWithCreative = true;

    private MilestoneManager manager;

    private void Start() {
        this.manager = GameObject.FindObjectOfType<MilestoneManager>();
        if(this.manager == null) {
            Debug.LogWarning("No MilestoneManager Component could be found");
        }
    }

    private void Update() {
        if(this.targetObject != null && this.milestone != null && this.manager != null) {
            bool enabled = this.milestone.isUnlocked || this.enableWithCreative;
            if(this.invertResult) {
                enabled = !enabled;
            }

            if(this.targetObject.activeSelf != enabled) {
                this.targetObject.SetActive(enabled);
            }
        }
    }
}
