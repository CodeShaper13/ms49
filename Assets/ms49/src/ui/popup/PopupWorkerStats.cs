using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class PopupWorkerStats : PopupWindow {

    [SerializeField]
    private TMP_Text _textHeader = null;
    [SerializeField]
    private TMP_Text _textInfo = null;
    [SerializeField]
    private FaceUiPreview preview = null;
    [SerializeField, Required]
    private ProgressBar _sliderEnergy = null;
    [SerializeField, Required]
    private ProgressBar _sliderHunger = null;
    [SerializeField, Required]
    private ProgressBar _sliderTemperature = null;
    [SerializeField, Required]
    private ProgressBar _sliderHappiness = null;

    private EntityWorker worker;
    private string infoTextTemplate;

    private void Awake() {
        this.infoTextTemplate = this._textInfo.text;
    }

    public void SetWorker(EntityWorker worker) {
        this.worker = worker;

        WorkerInfo info = this.worker.info;

        this._textHeader.text = info.FullName;

        this._textInfo.text = string.Format(this.infoTextTemplate,
            info.FullName, // Name
            this.worker.type.typeName, // Job
            info.pay, // Pay
            info.personality.displayName); // Personality

        this.preview.setTarget(this.worker);
    }

    protected override void onUpdate() {
        base.onUpdate();

        if(this.worker != null) {
            this.UpdateProgressBar(this._sliderEnergy, this.worker.energy);
            this.UpdateProgressBar(this._sliderHunger, this.worker.hunger);
            this.UpdateProgressBar(this._sliderTemperature, this.worker.temperature);
            this.UpdateProgressBar(this._sliderHappiness, this.worker.happiness);
        }
    }

    public void Callback_LocateWorker() {
        if(this.worker != null) {
            CameraController.instance.followTarget(this.worker);
        }
        this.Close();
    }

    public void Callback_FireWorker() {
        if(this.worker != null) {
            this.worker.world.entities.Remove(this.worker);
        }
        this.Close();
    }

    private void UpdateProgressBar(ProgressBar progressBar, UnlockableStat stat) {
        progressBar.transform.parent.gameObject.SetActive(stat.isStatEnabled());

        if(stat.isStatEnabled()) {
            progressBar.minValue = stat.minValue;
            progressBar.maxValue = stat.maxValue;
            progressBar.Value = stat.value;
        }
    }
}
