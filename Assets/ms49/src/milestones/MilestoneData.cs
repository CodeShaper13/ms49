using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Milestone", menuName = "MS49/Milestone/Milestone", order = 1)]
public class MilestoneData : ScriptableObject, ISerializationCallbackReceiver {

    [SerializeField]
    private string _milestoneName = null;
    [SerializeField]
    private MilestoneRequirerment[] _requirements = null;
    [SerializeField, Tooltip("If true, a layer is unlocked when completing this milestone")]
    private bool _unlocksLayer = false;
    [SerializeField, HideInInspector]
    private List<BuildableBase> _unlockedBuildables = new List<BuildableBase>();
    [SerializeField]
    private WorkerType[] _unlockedWorkerTypes = null;
    [SerializeField, Min(0)]
    private int _candaditeCountIncrease = 0;

    public string milestoneName => this._milestoneName;
    public MilestoneRequirerment[] requirements => this._requirements;
    public bool unlocksLayer => this._unlocksLayer;
    public bool isUnlocked { get; set; }
    public List<BuildableBase> unlockedBuildables => this._unlockedBuildables;
    public WorkerType[] unlockedWorkerTypes => this._unlockedWorkerTypes;
    public int hireCandaditeIncrease => this._candaditeCountIncrease;

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


#if UNITY_EDITOR
    [CustomEditor(typeof(MilestoneData))]
    public class PopupBuildEditor : Editor {

        private ReorderableList list;

        public void OnEnable() {
            this.list = new ReorderableList(
                this.serializedObject,
                this.serializedObject.FindProperty("_unlockedBuildables"));

            this.list.drawElementCallback = (rect, index, isActive, isFocused) => {
                EditorGUI.ObjectField(
                    new Rect(
                        rect.x,
                        rect.y,
                        300,
                        EditorGUIUtility.singleLineHeight),
                    list.serializedProperty.GetArrayElementAtIndex(index));
            };

            this.list.drawHeaderCallback = (rect) => {
                EditorGUI.LabelField(rect, "Unlocked Buildables");
            };
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            this.serializedObject.Update();
            this.list.DoLayoutList();
            this.serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(this.target);
        }
    }
#endif
}
