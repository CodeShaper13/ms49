using UnityEngine;
using TMPro;

public class TextMoney : MonoBehaviour {

    [SerializeField]
    private Animator _animator = null;
    [SerializeField]
    private TMP_Text _text = null;
    [SerializeField]
    private IntVariable _money = null;
    [SerializeField]
    private ColorVariable _colorMoneyPositive = null;
    [SerializeField]
    private ColorVariable _colorMoneyNegative = null;

    private int moneyLastFrame;

    private void LateUpdate() {
        this._text.text = string.Format("${0}", this._money.value);
        this._text.color = this._money.value >= 0 ? this._colorMoneyPositive : this._colorMoneyNegative;

        if(this._animator != null) {
            if(this._money.value > this.moneyLastFrame) {
                this._animator.Play("MoneyIncreaseClip");
            } else if(this._money.value < this.moneyLastFrame) {
                this._animator.Play("MoneyDecreaseClip");
            }
        }

        this.moneyLastFrame = this._money.value;
    }
}
