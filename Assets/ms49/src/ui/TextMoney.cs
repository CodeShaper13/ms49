using UnityEngine.UI;
using UnityEngine;

public class TextMoney : MonoBehaviour {

    [SerializeField]
    private Text text = null;

    private void Update() {
        this.text.text = "$" + Money.get();
        this.text.color = Money.inBlack() ? Color.green : Color.red;
    }
}
