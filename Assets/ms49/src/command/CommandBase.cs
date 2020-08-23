using System;
using UnityEngine;

public abstract class CommandBase : MonoBehaviour {

    [SerializeField, Tooltip("The name that's typed into the command line")]
    private string _commandName = null;
    [SerializeField, Tooltip("The commands syntax")]
    private string _syntax = "nul";
    [SerializeField]
    private string _description = "nul";

    protected CmdManager cmdManager { get; private set; }

    public string commandName { get { return this._commandName; } }
    public string syntax { get { return this._syntax; } }
    public string description { get { return this._description; } }

    private void Awake() {
        this.cmdManager = this.GetComponentInParent<CmdManager>();
    }

    /// <summary>
    /// Prints a message out to the chat log.
    /// </summary>
    public void logMessage(string message) {
        this.cmdManager.logMsg(message);
    }

    /// <summary>
    /// Called to run the command.  Args is a string array of every command arg, not including the command name.
    /// Return a string with a message to print out, or null for nothing.
    /// </summary>
    public abstract string runCommand(World world, string[] args);

    protected int parseInt(string[] args, uint index) {
        if(index >= args.Length) {
            throw new WrongSyntaxException();
        }

        int arg;
        if(!Int32.TryParse(args[index], out arg)) {
            throw new WrongSyntaxException();
        }

        return arg;
    }

    protected string parseString(string[] args, uint index) {
        if(index >= args.Length) {
            throw new WrongSyntaxException();
        }

        return args[index];
    }
}