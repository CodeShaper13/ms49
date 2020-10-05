using UnityEngine;
using UnityEngine.UI;

public class WorkerListButton : MonoBehaviour {

    [SerializeField]
    private Text textName = null;
    [SerializeField]
    private FaceUiPreview preview = null;

    public EntityWorker worker { get; private set; }

    public void setWorker(EntityWorker worker) {
        this.worker = worker;

        this.textName.text = worker.info.fullName;

        if(this.preview != null) {
            this.preview.setTarget(worker);
        }
    }

    public void callback_click() {
        PopupWorkerStats popup = Main.instance.findPopup<PopupWorkerStats>();
        if(popup != null) {
            popup.openAdditive();
            popup.setWorker(this.worker);
        }
    }

    public void callback_mapPin() {
        CameraController.instance.followTarget(this.worker.transform);
    }
}
