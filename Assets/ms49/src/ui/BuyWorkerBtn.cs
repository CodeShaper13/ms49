using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuyWorkerBtn : MonoBehaviour {

    public int cost;
    public int workersGiven;

    private Button btn;

    private void Awake() {
        Text text = this.GetComponentInChildren<Text>();
        text.text = this.workersGiven + " Workers ($" + this.cost + ")";

        this.btn = this.GetComponent<Button>();
    }

    private void Update() {
        this.btn.interactable = Money.get() >= this.cost;
    }

    public void callback() {
        this.GetComponentInParent<PopupBuyWorkers>().btnCallback(this);
    }
}
