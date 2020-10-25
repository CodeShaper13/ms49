using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class TaskLowerStat : TaskBase<EntityWorker> {
    
    [SerializeField]
    private UnlockableStat stat = null;
    [SerializeField]
    private bool onlyExecuteWhenAwake = false;

    [Space]

    [SerializeField]
    private bool randomDecrease = false;
    [SerializeField, Tooltip("If -1, a random value from range will be used to decrease the stat each frame.")]
    [HideInInspector]
    private float decreaseRate = 0;
    [SerializeField, MinMaxSlider(0, 10)]
    [HideInInspector]
    private Vector2 range = new Vector2(1, 2);

    public override bool continueExecuting() {
        if(this.onlyExecuteWhenAwake) {
            return !this.owner.isSleeping;
        }
        return true;
    }

    public override void preform() {
        float amount;

        if(this.randomDecrease) {
            amount = Random.Range(this.range.x, this.range.y);
        } else {
            amount = this.decreaseRate;
        }

        this.stat.decrease(amount * Time.deltaTime);
    }

    public override bool shouldExecute() {
        return this.stat != null && this.stat.isStatEnabled() && (this.onlyExecuteWhenAwake != this.owner.isSleeping);
    }

    public override bool allowLowerPriority() {
        return true;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TaskLowerStat), true)]
    [CanEditMultipleObjects]
    public class TaskLowerStatEditor : Editor {

        private SerializedProperty randomDecrease;
        private SerializedProperty decreaseRate;
        private SerializedProperty range;

        private void OnEnable() {
            this.randomDecrease = this.serializedObject.FindProperty("randomDecrease");
            this.decreaseRate = this.serializedObject.FindProperty("decreaseRate");
            this.range = this.serializedObject.FindProperty("range");
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            this.DrawDefaultInspector();

            if(randomDecrease.boolValue) {
                EditorGUILayout.PropertyField(this.range);
            } else {
                EditorGUILayout.PropertyField(this.decreaseRate);
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
