using UnityEngine;
using UnityEngine.UI;

public class PopupSign : PopupWindow {

    [SerializeField]
    private InputField inputField = null;

    private CellBehaviorSign behavior;

    public void setMeta(CellBehaviorSign behavior) {
        this.behavior = behavior;
        this.inputField.text = behavior.message;
    }

    public void callback_ok() {
        if(this.behavior != null) {
            this.behavior.message = this.inputField.text;
        }

        this.Close();
    }
}
