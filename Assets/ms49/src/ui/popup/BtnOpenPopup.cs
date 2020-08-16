using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BtnOpenPopup : MonoBehaviour {

    [SerializeField]
    private PopupWindow popup = null;
    [SerializeField, Tooltip("If set, the popup will be opened with this hotkey")]
    private KeyCode hotkey = KeyCode.None;

    private void Awake() {
        this.GetComponent<Button>().onClick.AddListener(() => {
            this.openIfSet();
        });
    }

    private void Update() {
        if(Input.GetKeyDown(this.hotkey)) {
            this.openIfSet();
        }
    }

    private void openIfSet() {
        if(this.popup != null) {
            this.popup.open();
        }
    }
}
