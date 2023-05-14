public class CommandSpawn : CommandBase {

    public override string runCommand(World world, string[] args) {
        int id = this.parseInt(args, 0);
        int x = this.parseInt(args, 1);
        int y = this.parseInt(args, 2);
        int depth;
        if(args.Length > 3) {
            depth = this.parseInt(args, 3);
        } else {
            depth = CameraController.instance.currentLayer;
        }

        world.entities.Spawn(new Position(x, y, depth), id);

        return "spawned entity";
    }
}
