using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CmdWindow : MonoBehaviour {

    [SerializeField]
    /// <summary> The parent of all the chat log objects. </summary>
    private Transform root = null;
    [SerializeField]
    private InputField inputField = null;
    [SerializeField]
    private Text outputField = null;
    [SerializeField]
    private CmdManager cmdManager = null;
    [SerializeField]
    private int maxLines = 15;
    [SerializeField]
    private KeyCode toggleWindowKey = KeyCode.Tilde;

    private bool isOpen = false;
    /// <summary> The number of lines to text in the log. </summary>
    private int textLines;

    private void Awake() {
        this.closeWindow();

        this.inputField.onEndEdit.AddListener(fieldValue => {
            if(Input.GetKeyDown(KeyCode.Return)) {
                this.cmdManager.tryRunCommand(fieldValue);
                this.clearInputLine();
                this.inputField.OnPointerClick(new PointerEventData(EventSystem.current));
            }
        });
    }

    private void Update() {
        if(Input.GetKeyDown(this.toggleWindowKey)) {
            if(this.isOpen) {
                this.closeWindow();
            } else {
                this.openWindow();
            }
        }
    }

    private void OnEnable() {
        this.clearInputLine();
    }

    public void openWindow() {
        this.isOpen = true;
        this.root.gameObject.SetActive(true);

        // Make the input field selected.
        //inputField.OnPointerClick(new PointerEventData(EventSystem.current));
        //EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
    }

    public void closeWindow() {
        this.isOpen = false;
        this.clearInputLine();
        this.root.gameObject.SetActive(false);
    }

    /// <summary>
    /// Prints a message out to the chat log.
    /// </summary>
    public void logMessage(string message) {
        this.textLines++;
        if(this.textLines > this.maxLines) {
            // Remove oldest line
            string s = this.outputField.text;
            this.outputField.text = s.Substring(s.IndexOf('\n') + 1);
            textLines--;
        }

        this.outputField.text += (message + '\n');
    }

    /// <summary>
    /// Makes the input line blank, or "".
    /// </summary>
    private void clearInputLine() {
        this.inputField.text = string.Empty;
    }
}
