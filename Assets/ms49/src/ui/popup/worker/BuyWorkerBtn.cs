using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuyWorkerBtn : MonoBehaviour {

    [SerializeField]
    private int entityId = 1;

    private PopupBuyWorkers popup;
    private Button btn;

    private void Awake() {
        this.popup = this.GetComponentInParent<PopupBuyWorkers>();
        this.btn = this.GetComponent<Button>();

        Text text = this.GetComponentInChildren<Text>();
        text.text = text.text + " ($" + this.popup.workerCost + ")";
    }

    private void Update() {
        this.btn.interactable = this.popup.money.value >= this.popup.workerCost;
    }

    public void callback() {
        this.GetComponentInParent<PopupBuyWorkers>().btnCallback(this.entityId);
    }
}
