using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AiManager {

    private List<TaskListEntry> tasks;

    public AiManager() {
        this.tasks = new List<TaskListEntry>();
    }

    /// <summary>
    /// Adds a task to the Ai Manager.
    /// </summary>
    public void addTask(int priority, ITask task) {
        this.tasks.Add(new TaskListEntry(priority, task));

        this.tasks = this.tasks.OrderBy(e => e.priority).ToList();
    }

    /// <summary>
    /// Stops all running tasks and invokes the resetTask() method on them.
    /// </summary>
    public void stopAllTasks() {
        foreach(TaskListEntry entry in this.tasks) {
            if(entry.isRunning()) {
                entry.task.resetTask();
                entry.setRunning(false);
            }
        }
    }

    /// <summary>
    /// This should be called every frame to update all of the AI tasks.
    /// </summary>
    public void updateAi() {
        bool terminateRest = false;

        for(int i = 0; i < this.tasks.Count; i++) {
            TaskListEntry entry = this.tasks[i];
            ITask task = entry.task;

            if(terminateRest) {
                if(entry.isRunning()) {
                    //this.character.StopCoroutine("preform");
                    entry.task.resetTask();
                    entry.setRunning(false);
                }
                continue;
            }

            if(entry.isRunning()) {
                // Task is running
                if(!task.continueExecuting()) {
                    // End task.
                    entry.setRunning(false);
                    //this.character.StopCoroutine("preform");
                    task.resetTask();
                }
            } else {
                // Task is not running
                if(task.shouldExecute()) {
                    entry.setRunning(true);
                    task.startExecute();
                    //this.character.StartCoroutine(task.preform()); // mut be called ater others have been shut down
                }
            }

            if(entry.isRunning()) {
                if(!task.allowLowerPriority()) {
                    terminateRest = true;
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
        public float timeRunning { get; private set; }

        private bool _isRunning;

        public TaskListEntry(int i, ITask task) {
            this.priority = i;
            this.task = task;
            this.setRunning(false);
        }

        public void setRunning(bool running) {
            if(running != this._isRunning) {
                this.timeRunning = 0;
            }

            this._isRunning = running;
        }

        public bool isRunning() {
            return this._isRunning;
        }
    }
}
