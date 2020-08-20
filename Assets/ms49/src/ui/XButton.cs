using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Closes a Popup to close when a Button is clicked.
/// When attached to GameObjects with a Button, this will add a
/// callback to the Button's onClick listener that closes the first
/// Popup found in a parent object.
/// </summary>
[RequireComponent(typeof(Button))]
public class XButton : MonoBehaviour {

    private void Awake() {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(() => callback());
    }

    public void callback() {
        PopupWindow popup = this.GetComponentInParent<PopupWindow>();
        if(popup != null) {
            popup.close();
        }
    }
}
