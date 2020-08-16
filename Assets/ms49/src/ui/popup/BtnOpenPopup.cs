using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BtnOpenPopup : MonoBehaviour {

    [SerializeField]
    private PopupWindow popup = null;

    private void Awake() {
        this.GetComponent<Button>().onClick.AddListener(() => {
            if(this.popup != null) {
                this.popup.open();
            }
        });
    }
}
