using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldSaveName : MonoBehaviour {

    [SerializeField]
    private StringVariable _regexPattern = null;
    [SerializeField]
    private Image imageInvalidIcon = null;

    private TMP_InputField inputField;

    public bool isValidName { get; private set; }
    public string text {
        get => this.inputField.text;
        set { this.inputField.text = value; }
    }

    private void Awake() {
        this.inputField = this.GetComponent<TMP_InputField>();
        this.inputField.onValueChanged.AddListener(this.OnValueChanged);
    }

    private void OnValueChanged(string text) {
        // Remove invalid characters
        if(this._regexPattern != null) {
            this.inputField.text = Regex.Replace(this.inputField.text, this._regexPattern.value, string.Empty);
        }

        // Update the create button interactability
        bool validName = !string.IsNullOrEmpty(this.inputField.text);
        foreach(SaveFile save in Main.instance.GetAllSaves()) {
            if(save.saveName == this.inputField.text) {
                validName = false;
                break;
            }
        }

        this.isValidName = validName;

        // Update the invalid exclamation icon.
        if(this.imageInvalidIcon != null) {
            this.imageInvalidIcon.enabled = !this.isValidName;
        }
    }
}
