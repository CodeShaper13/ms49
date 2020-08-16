﻿using UnityEngine;
using System.Collections;

public interface ITask {

    /// <summary>
    /// Called every frame the task is not running.
    /// If true is returned, that task will start running that frame.
    /// </summary>
    bool shouldExecute();

    /// <summary>
    /// Called when the task first starts executing.
    /// </summary>
    void startExecute();

    /// <summary>
    /// Called every frame that the task is running to continue "running" it.
    /// </summary>
    void preform();

    /// <summary>
    /// Called every frame to determine if the AI should execute.
    /// </summary>
    bool continueExecuting();

    /// <summary>
    /// Called when the task has stopped running.  This should reset the
    /// task, so it can be run again later on.
    /// 
    /// This is also called from the constructor.
    /// </summary>
    void resetTask();

    bool allowLowerPriority();
}