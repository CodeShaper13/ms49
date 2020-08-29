using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ButtonBase : MonoBehaviour {

    private void Awake() {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(() => callback());
    }

    public abstract void callback();
}
