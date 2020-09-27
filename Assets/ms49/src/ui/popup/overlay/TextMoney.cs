using UnityEngine.UI;
using UnityEngine;

public class TextMoney : MonoBehaviour {

    [SerializeField]
    private Animator anim = null;
    [SerializeField]
    private Text text = null;
    [SerializeField]
    private IntVariable money = null;
    [SerializeField]
    private ColorVariable colorMoneyOk = null;
    [SerializeField]
    private ColorVariable colorMoneyNotEnough = null;

    private int moneyLastFrame;

    private void Update() {
        this.text.text = "$" + this.money.value;
        this.text.color = this.money.value >= 0 ? this.colorMoneyOk : this.colorMoneyNotEnough;

        if(this.anim != null) {
            if(this.money.value > this.moneyLastFrame) {
                this.anim.Play("MoneyIncreaseClip");
            } else if(this.money.value < this.moneyLastFrame) {
                this.anim.Play("MoneyDecreaseClip");
            }
        }

        this.moneyLastFrame = this.money.value;
    }
}
