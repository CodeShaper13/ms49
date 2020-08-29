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
    private Image img = null;

    private EntityWorker worker;
    private string infoTextTemplate;

    protected override void initialize() {
        base.initialize();

        this.infoTextTemplate = this.infoText.text;
    }

    public void setWorker(EntityWorker worker) {
        this.worker = worker;

        WorkerStats stats = this.worker.stats;

        this.nameText.text = stats.getFullName().ToLower();

        this.infoText.text = string.Format(this.infoTextTemplate, stats.getFullName(), this.worker.typeName, "todo");
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.worker != null) {
            // Update face.
            this.img.sprite = this.worker.animator.getSprite();

            StringBuilder sb = new StringBuilder();
            this.worker.writeWorkerInfo(sb);
            this.statsText.text = sb.ToString();
        }
    }

    public void callback_fire() {
        this.worker.world.entities.remove(this.worker);
        this.close();
    }
}
