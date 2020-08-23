public class CommandHelp : CommandBase {
    
    public override string runCommand(World world, string[] args) {
        this.logMessage("Commands:");
        foreach(CommandBase command in this.cmdManager.commands) {
            this.logMessage(
                command.commandName +
                "  " +
                command.syntax +
                ":  "
                + command.description);
        }
        return null;
    }
}
