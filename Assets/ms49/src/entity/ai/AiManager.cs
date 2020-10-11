using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AiManager : MonoBehaviour {

    private List<TaskListEntry> tasks;

    [SerializeField, Header("Read only, used for finding the current task")]
    [TextArea(1, 100)]
    private string currentTask;

    private void Awake() {
        this.tasks = new List<TaskListEntry>();
    }

    private void Start() {
        foreach(ITask task in this.GetComponentsInChildren<ITask>()) {
            this.tasks.Add(new TaskListEntry(task.priority, task));
        }

        this.tasks = this.tasks.OrderBy(e => e.priority).ToList();
    }

    private void LateUpdate() {
        this.currentTask = this.generateDebugText();
    }

    /// <summary>
    /// Stops all running tasks and invokes the resetTask() method on them.
    /// </summary>
    public void stopAllTasks() {
        foreach(TaskListEntry entry in this.tasks) {
            if(entry.isRunning()) {
                entry.stopRunning();
            }
        }
    }

    /// <summary>
    /// This should be called every frame to update all of the AI tasks.
    /// </summary>
    public void updateAi() {
        for(int i = 0; i < this.tasks.Count; i++) {
            TaskListEntry entry = this.tasks[i];
            ITask task = entry.task;

            if(entry.isRunning()) {
                // Task is running
                if(task.continueExecuting()) {
                    if(!task.allowLowerPriority()) {
                        break;
                    }
                } else {
                    // For whatever reason the task no longer wants to run, stop it
                    entry.stopRunning();
                }
            } else {
                // Task is not running
                if(task.shouldExecute()) {
                    // If the task being looked at doesn't allow
                    // lower priority, stop all others.
                    if(!task.allowLowerPriority()) {
                        for(int j = i + 1; j < this.tasks.Count; j++) {
                            this.tasks[j].stopRunning();
                        }
                    }

                    entry.startRunning();

                    if(!task.allowLowerPriority()) {
                        break;
                    }
                }
            }
        }

        foreach(TaskListEntry entry in this.tasks) {
            if(entry.isRunning()) {
                entry.task.preform();
            }
        }
    }

    public string generateDebugText() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Running Task(s):");
        int lineCount = 0;
        foreach(TaskListEntry e in this.tasks) {
            if(e.isRunning()) {
                lineCount++;
                sb.AppendLine("    " + e.task.ToString());
            }
        }
        if(lineCount == 0) {
            sb.AppendLine("   NO TASKS");
        }
        return sb.ToString();
    }

    private class TaskListEntry {

        public readonly int priority;
        public readonly ITask task;

        private bool _isRunning;

        public TaskListEntry(int i, ITask task) {
            this.priority = i;
            this.task = task;
        }

        public void startRunning() {
            if(!this._isRunning) {
                this.task.startExecute();
                //this.task.startPreformCoroutine();

                this._isRunning = true;
            }
        }

        public void stopRunning() {
            if(this._isRunning) {
                //this.task.stopCoroutine();
                this.task.onTaskStop();

                this._isRunning = false;
            }
        }

        public bool isRunning() {
            return this._isRunning;
        }
    }
}
