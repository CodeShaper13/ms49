public class EntityBuilder : EntityWorker {

    public override void initialize(World world, int id, int depth) {
        base.initialize(world, id, depth);

        this.aiManager.addTask(5, new TaskBuild(this, this.moveHelper));
    }
}
