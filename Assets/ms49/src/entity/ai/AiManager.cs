using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AiManager : MonoBehaviour {

    private List<TaskListEntry> tasks;

    private void Awake() {
        this.tasks = new List<TaskListEntry>();
    }

    private void Start() {
        foreach(ITask task in this.GetComponentsInChildren<ITask>()) {
            this.tasks.Add(new TaskListEntry(task.priority, task));
        }

        this.tasks = this.tasks.OrderBy(e => e.priority).ToList();
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

        int startIndex = 0;
        for(int k = 0; k < this.tasks.Count; k++) {
            TaskListEntry entry = this.tasks[k];
            ITask task = entry.task;

            if(entry.isRunning() && !task.canBeInterupted()) {
                startIndex = k;
                break;
            }
        }

        for(int i = startIndex; i < this.tasks.Count; i++) {
            TaskListEntry entry = this.tasks[i];
            ITask task = entry.task;

            if(entry.isRunning()) {
                // Task is running

                // Check is the task should continue executing
                if(task.continueExecuting()) {
                    // Tasks should continue.

                    if(!task.allowLowerPriority()) {
                        // Don't let tasks with a lower priority run 
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

    public void generateDebugText(StringBuilder sb, string indent) {
        sb.AppendLine("Running AI Task(s):");
        int lineCount = 0;
        foreach(TaskListEntry e in this.tasks) {
            if(e.isRunning()) {
                lineCount++;
                sb.AppendLine(indent + e.task.ToString());
            }
        }
        if(lineCount == 0) {
            sb.AppendLine(indent + "NO TASKS");
        }
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
                this.task.onTaskStart();
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
