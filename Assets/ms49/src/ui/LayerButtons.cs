using UnityEngine;
using UnityEngine.UI;

public class LayerButtons : MonoBehaviour {

    [SerializeField]
    private Button upBtn = null;
    [SerializeField]
    private Button downBtn = null;
    [SerializeField]
    private Text text = null;

    private void Update() {
        this.updateText();
    }

    public void callback(bool moveUp) {
        CameraController cc = CameraController.instance;

        cc.changeLayer(cc.currentLayer + (moveUp ? -1 : 1));

        // Update button interactability
        if(cc.currentLayer == 0) {
            this.upBtn.interactable = false;
        } else {
            this.upBtn.interactable = true;
        }

        if(cc.currentLayer == 10) {
            this.downBtn.interactable = false;
        } else {
            this.downBtn.interactable = true;
        }
    }

    private void updateText() {
        int i = CameraController.instance.currentLayer;
        if(Main.DEBUG) {
            this.text.text = i.ToString();
        }
        else {
            this.text.text = (i == 0 ? "surface" : "-" + (i * 100) + " feet");
        }
    }
}
