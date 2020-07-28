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
    
    public void setWorker(EntityWorker worker) {
        this.worker = worker;

        WorkerStats stats = this.worker.stats;

        this.nameText.text = (stats.getGender() == EnumGender.MALE ? "Mr. " : "Miss") + stats.getLastName();

        this.infoText.text = "Name: " + stats.getFullName() + "\nAge: " + 0; 
    }

    public override void onUpdate() {
        if(this.worker != null) {
            // Update face.
            this.img.sprite = this.worker.faces.getFace();

            StringBuilder sb = new StringBuilder();

            this.worker.writeWorkerInfo(sb);

            this.statsText.text = sb.ToString();
        }
    }
}
