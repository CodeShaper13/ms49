using UnityEngine;
using UnityEngine.UI;

public class WorkerListButton : MonoBehaviour {

    [SerializeField]
    private Text textName = null;

    public EntityWorker worker { get; private set; }

    public void setWorker(EntityWorker worker) {
        this.worker = worker;

        this.textName.text = worker.info.fullName;
    }

    public void callback_click() {
        PopupWorkerStats popup = Main.instance.findPopup<PopupWorkerStats>();
        if(popup != null) {
            popup.open(false);
            popup.setWorker(this.worker);
        }
    }

    public void callback_mapPin() {
        CameraController.instance.setCameraPosSmooth(this.worker.worldPos);
    }
}
