using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class WorkerListButton : MonoBehaviour {

    [SerializeField, Required]
    private TMP_Text textName = null;
    [SerializeField, Required]
    private FaceUiPreview preview = null;

    public EntityWorker worker { get; private set; }

    public void setWorker(EntityWorker worker) {
        this.worker = worker;

        this.textName.text = worker.info.FullName;
        this.preview.setTarget(worker);
    }

    public void Callback_ViewDetailedStats() {
        PopupWorkerStats popup = Main.instance.FindPopup<PopupWorkerStats>();
        if(popup != null) {
            popup.openAdditive();
            popup.SetWorker(this.worker);
        }
    }

    public void Callback_FollowWorker() {
        CameraController.instance.followTarget(this.worker);
    }
}
