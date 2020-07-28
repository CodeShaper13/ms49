using System.Collections.Generic;
using System.Linq;

public class AiManager<T> where T : EntityWorker {

    private List<TaskListEntry> tasks;
    private T character;

    public AiManager(T character) {
        this.tasks = new List<TaskListEntry>();
        this.character = character;
    }

    /// <summary>
    /// Adds a task to the Ai Manager.
    /// </summary>
    public void addTask(int priority, ITask task) {
        this.tasks.Add(new TaskListEntry(priority, task));

        this.tasks = this.tasks.OrderBy(e => e.priority).ToList();
    }

    /// <summary>
    /// This should be called every frame to update all of the AI tasks.
    /// </summary>
    public void updateAi() {
        foreach(TaskListEntry entry in this.tasks) {
            ITask task = entry.task;

            if(entry.isRunning) {
                // Task is running
                if(!task.continueExecuting()) {
                    entry.isRunning = false;
                    task.resetTask();
                }
            } else {
                // Task is not running
                if(task.shouldExecute()) {
                    entry.isRunning = true;
                    task.startExecute();
                }
            }
        }

        foreach(TaskListEntry entry in this.tasks) {
            if(entry.isRunning) {
                entry.task.preform();
                if(!entry.task.allowLowerPriority()) {
                    break;
                }
            }
        }
    }

    private class TaskListEntry {

        public readonly int priority;
        public readonly ITask task;

        public bool isRunning;

        public TaskListEntry(int i, ITask task) {
            this.priority = i;
            this.task = task;
            this.isRunning = false;
        }
    }
}
