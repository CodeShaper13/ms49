using UnityEngine;
using UnityEngine.UI;

public class PauseGrayBackground : MonoBehaviour {

    [SerializeField]
    private Image _img = null;

    private void OnEnable() {
        this._img.enabled = Main.instance.isPlaying();
    }
}
