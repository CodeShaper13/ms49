using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Causes a Popup to open when a Button is clicked or a hotkey is
/// pressed.
/// When attached to GameObjects with a Button, this will add a
/// callback to the Button's onClick listener that opens a Popup.
/// </summary>
[RequireComponent(typeof(Button)), DisallowMultipleComponent]
public class BtnOpenPopup : MonoBehaviour {

    [SerializeField]
    private PopupWindow popup = null;
    [SerializeField, Tooltip("If set, the popup will be opened with this hotkey")]
    private KeyCode hotkey = KeyCode.None;

    private void Start() {
        this.GetComponent<Button>().onClick.AddListener(() => {
            this.openPopupIfSet();
        });
    }

    private void Update() {
        if(Input.GetKeyDown(this.hotkey)) {
            this.openPopupIfSet();
        }
    }

    private void openPopupIfSet() {
        if(this.popup != null) {
            this.popup.open();
        }
    }
}
