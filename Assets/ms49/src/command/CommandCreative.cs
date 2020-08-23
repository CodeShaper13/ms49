using UnityEngine;

public class CommandCreative : CommandBase {

    public override string runCommand(World world, string[] args) {
        string s = this.parseString(args, 0);
        if(s == "on") {
            CameraController.instance.creative = true;
        }
        else if(s == "off") {
            CameraController.instance.creative = false;
        }
        else {
            throw new WrongSyntaxException();
        }

        return "toggling";
    }
}
