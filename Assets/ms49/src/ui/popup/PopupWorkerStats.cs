using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PopupWorkerStats : PopupWindow {

    [SerializeField]
    private Text nameText = null;
    [SerializeField]
    private Text infoText = null;
    [SerializeField]
    private Text statsText = null;
    [SerializeField]
    private FaceUiPreview preview = null;

    private EntityWorker worker;
    private string infoTextTemplate;

    protected override void initialize() {
        base.initialize();

        this.infoTextTemplate = this.infoText.text;
    }

    public void setWorker(EntityWorker worker) {
        this.worker = worker;

        WorkerInfo info = this.worker.info;

        this.nameText.text = (info.firstName + " " + info.lastName).ToLower();

        this.infoText.text = string.Format(this.infoTextTemplate,
            this.worker.type.typeName,
            info.pay,
            Main.instance.personalities.getDescription(info.personality));
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.worker != null) {
            this.preview.setTarget(this.worker);

            StringBuilder sb = new StringBuilder();
            this.worker.writeWorkerInfo(sb);
            this.statsText.text = sb.ToString();
        }
    }

    /// <summary>
    /// Centers the Camera on the Worker and closes the UI.
    /// </summary>
    public void callback_find() {
        if(this.worker != null) {
            CameraController.instance.setCameraPos(this.worker.worldPos);
        }
        this.close();
    }

    public void callback_fire() {
        if(this.worker != null) {
            this.worker.world.entities.remove(this.worker);
        }
        this.close();
    }
}
