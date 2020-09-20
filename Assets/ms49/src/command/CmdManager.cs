using UnityEngine;

public class CmdManager : MonoBehaviour {

    [SerializeField]
    private CmdWindow cmdWindow = null;

    public CommandBase[] commands { get; private set; }

    private void Awake() {
        this.commands = this.GetComponentsInChildren<CommandBase>();
    }

    /// <summary>
    /// Tries to run a command.  Line is the contents of the command line.
    /// </summary>
    /// <param name="line"></param>
    public void tryRunCommand(string line) {
        line = line.Trim();

        int j = line.IndexOf(' ');

        string cmdName;
        string[] args;

        if(j == -1) {
            cmdName = line;
            args = new string[0];
        }
        else {
            cmdName = line.Substring(0, j);
            args = line.Substring(j + 1).Split(' ');
        }

        foreach(CommandBase cmd in this.commands) {
            if(string.IsNullOrEmpty(cmd.commandName)) {
                continue; // Name is blank, probably never set in the inspector.
            }

            if(cmdName == cmd.commandName) {
                try {
                    World world = GameObject.FindObjectOfType<World>();

                    string result = cmd.runCommand(world, args);
                    if(result != null) {
                        this.logMsg(result);
                    }
                }
                catch(WrongSyntaxException) {
                    this.logMsg("Wrong arguments, try: " + cmd.commandName + " " + cmd.syntax);
                }
                return;
            }
        }
        this.logMsg("Command \"" + cmdName + "\" could not be found.  Try \"help\" for a list of commands");
    }

    public void logMsg(string s) {
        if(this.cmdWindow != null) {
            this.cmdWindow.logMessage(s);
        }
    }
}
