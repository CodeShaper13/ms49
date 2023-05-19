using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Closes a Popup to close when a Button is clicked.
/// When attached to GameObjects with a Button, this will add a
/// callback to the Button's onClick listener that closes the first
/// Popup found in a parent object.
/// </summary>
[RequireComponent(typeof(Button))]
public class XButton : ButtonBase {

    public override void callback() {
        PopupWindow popup = this.GetComponentInParent<PopupWindow>();
        if(popup != null) {
            popup.Close();
        }
    }
}
