using UnityEngine;
using UnityEngine.UI;

public class LayerButtons : MonoBehaviour {

    [SerializeField]
    private Button upBtn = null;
    [SerializeField]
    private Button downBtn = null;
    [SerializeField]
    private Text text = null;
    [SerializeField]
    private World world = null;

    private void Update() {
        this.updateText();
        this.updateButtonInteracability();
    }

    public void callback(bool moveUp) {
        CameraController cc = CameraController.instance;

        // Attempt to change the Layer
        cc.changeLayer(cc.currentLayer + (moveUp ? -1 : 1));
    }

    private void updateButtonInteracability() {
        CameraController cc = CameraController.instance;

        if(cc.currentLayer == 0) {
            this.upBtn.interactable = false;
        } else {
            this.upBtn.interactable = true;
        }

        if(this.world.isDepthUnlocked(cc.currentLayer + 1) || CameraController.instance.inCreativeMode) {
            this.downBtn.interactable = true;
        } else {
            this.downBtn.interactable = false;
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
