public class CommandLayer : CommandBase {

    public override string runCommand(World world, string[] args) {
        int layer = this.parseInt(args, 0);
        CameraController.instance.changeLayer(layer, false);

        return "attempting to go to layer " + layer;
    }
}
