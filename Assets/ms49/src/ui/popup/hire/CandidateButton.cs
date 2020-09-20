using UnityEngine;
using UnityEngine.UI;

public class CandidateButton : MonoBehaviour {

    [SerializeField]
    private Text textName = null;
    [SerializeField]
    private Text textPay = null;
    [SerializeField]
    private Text textPersonality = null;
    [SerializeField]
    private Button buttonHire = null;
    [SerializeField]
    private IntVariable money = null;
    [SerializeField]
    private FaceUiPreview workerPreview = null;

    private Candidate candidate;
    private PopupHireWorkers popup;

    private void Awake() {
        this.popup = this.GetComponentInParent<PopupHireWorkers>();
    }

    private void Update() {
        this.buttonHire.interactable = this.money.value >= this.popup.world.hireCandidates.hireCost || CameraController.instance.inCreativeMode;
    }

    public void setCandidate(Candidate c) {
        this.candidate = c;

        if(this.candidate != null) {
            WorkerInfo info = this.candidate.info;

            this.textName.text = info.fullName;
            this.textPay.text = "$" + info.pay + " / Day";
            this.textPersonality.text = Main.instance.personalities.getDescription(info.personality);

            this.workerPreview.setTarget(
                info,
                candidate.type);
        } else {
            this.textName.text = "error, missing candidate";
        }
    }

    public void callback_hire(int buttonIndex) {
        if(!CameraController.instance.inCreativeMode) {
            this.money.value -= this.popup.world.hireCandidates.hireCost;
        }

        this.popup.world.hireCandidates.hireAndTrain(buttonIndex);

        this.popup.refreshCandidateButtons();
    }
}
