using UnityEngine;
using System.Collections;

public class TaskDestroy : TaskBase<EntityBuilder> {

    public TaskDestroy(EntityBuilder owner, MoveHelper moveHelper) : base(owner, moveHelper) { }

    public override bool continueExecuting() {
        throw new System.NotImplementedException();
    }

    public override void preform() {
        throw new System.NotImplementedException();
    }

    public override bool shouldExecute() {
        throw new System.NotImplementedException();
    }
}
