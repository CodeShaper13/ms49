using UnityEngine;
using UnityEngine.UI;

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
