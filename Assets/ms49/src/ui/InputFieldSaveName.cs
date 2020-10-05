using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class InputFieldSaveName : MonoBehaviour {

    [SerializeField]
    private string regexWorldName = @"[^a-zA-Z0-9_!]";
    [SerializeField]
    private Image imageInvalidIcon = null;
    [SerializeField]
    private InputField inputSaveName = null;

    public bool isValidName {
        get; private set;
    }
    public string text {
        get { return this.inputSaveName.text; }
        set { this.inputSaveName.text = value; }
    }

    public void callback_characterChange() {
        // Remove invalid characters
        this.inputSaveName.text = Regex.Replace(this.inputSaveName.text, this.regexWorldName, "");

        // Update the create button interactability
        bool validName = !string.IsNullOrEmpty(this.inputSaveName.text);
        foreach(SaveFile save in Main.instance.getAllSaves()) {
            if(save.saveName == this.inputSaveName.text) {
                validName = false;
                break;
            }
        }

        this.isValidName = validName;

        // Update the invalid exclamation icon.
        this.imageInvalidIcon.enabled = !this.isValidName;
    }
}
