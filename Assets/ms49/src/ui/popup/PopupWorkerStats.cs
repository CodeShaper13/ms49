using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PopupWorkerStats : PopupWindow {

    [SerializeField]
    private Text _headerText = null;
    [SerializeField]
    private Text infoText = null;
    [SerializeField]
    private FaceUiPreview preview = null;
    [SerializeField]
    private Slider _sliderEnergy = null;
    [SerializeField]
    private Slider _sliderHunger = null;
    [SerializeField]
    private Slider _sliderTemperature = null;
    [SerializeField]
    private Slider _sliderHappiness = null;

    private EntityWorker worker;
    private string infoTextTemplate;

    private void Start() {
        this.infoTextTemplate = this.infoText.text;
    }

    public void setWorker(EntityWorker worker) {
        this.worker = worker;

        WorkerInfo info = this.worker.info;

        this._headerText.text = info.lastName.ToLower();

        this.infoText.text = string.Format(this.infoTextTemplate,
            this.worker.info.fullName, // Name
            this.worker.type.typeName, // Job
            info.pay, // Pay
            info.personality.displayName); // Personality

        this.preview.setTarget(this.worker);
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.worker != null) {
            this.func(this._sliderEnergy, this.worker.energy);
            this.func(this._sliderHunger, this.worker.hunger);
            this.func(this._sliderTemperature, this.worker.temperature);
            this.func(this._sliderHappiness, this.worker.happiness);
        }
    }

    public void callback_fire() {
        if(this.worker != null) {
            this.worker.world.entities.Remove(this.worker);
        }
        this.close();
    }

    private void func(Slider slider, UnlockableStat stat) {
        slider.transform.parent.gameObject.SetActive(stat.isStatEnabled());

        if(stat.isStatEnabled()) {
            slider.minValue = stat.minValue;
            slider.maxValue = stat.maxValue;
            slider.value = stat.value;
        }
    }
}
