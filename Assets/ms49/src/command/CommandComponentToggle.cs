using UnityEngine;

public class CommandComponentToggle : CommandBase {

    [SerializeField]
    private GameObject component = null;

    public override string runCommand(World world, string[] args) {
        if(args.Length == 0) {
            this.component.SetActive(!this.component.activeSelf);
        }
        else if(args.Length == 1) {
            string s = args[0];
            if(s == "on") {
                this.component.SetActive(true);
            }
            else if(s == "off") {
                this.component.SetActive(false);
            }
            else {
                throw new WrongSyntaxException();
            }
        }
        else {
            throw new WrongSyntaxException();
        }

        return "toggling";
    }
}
