public class CommandCreative : CommandBase {

    public override string runCommand(World world, string[] args) {
        string s = this.parseString(args, 0);
        if(s == "on") {
            CameraController.instance.inCreativeMode = true;
            return "creative mode now on";
        }
        else if(s == "off") {
            CameraController.instance.inCreativeMode = false;
            return "creative mode now off";
        }
        else {
            throw new WrongSyntaxException();
        }
    }
}
