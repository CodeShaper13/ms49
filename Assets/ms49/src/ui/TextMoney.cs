using UnityEngine.UI;
using UnityEngine;

public class TextMoney : MonoBehaviour {

    [SerializeField]
    private Text text = null;
    [SerializeField]
    private IntVariable money = null;
    [SerializeField]
    private ColorVariable colorMoneyOk = null;
    [SerializeField]
    private ColorVariable colorMoneyNotEnough = null;

    private void Update() {
        this.text.text = "$" + this.money.value;
        this.text.color = this.money.value >= 0 ? this.colorMoneyOk : this.colorMoneyNotEnough;
    }
}
