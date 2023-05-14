using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHireWorkers : PopupWorldReference {

    [SerializeField]
    private RectTransform hireProgressRect = null;
    [SerializeField]
    private RectTransform candidateListRect = null;
    [SerializeField]
    private RectTransform btnArea = null;
    [SerializeField]
    private GameObject candidateButtonPrefab = null;
    [SerializeField]
    private Slider hireProgressSlider = null;
    [SerializeField]
    private Text hireCostText = null;

    private List<CandidateButton> candidateButtons;
    private bool refreshedCandidates;

    protected override void Start() {
        base.Start();

        this.candidateButtons = new List<CandidateButton>();
    }

    protected override void onOpen() {
        base.onOpen();

        this.hireCostText.text = string.Format(this.hireCostText.text, this.world.hireCandidates.hireCost);
    }

    protected override void onClose() {
        base.onClose();

        this.refreshedCandidates = false;
    }

    protected override void onUpdate() {
        bool isTraining = this.world.hireCandidates.isTraining();

        this.hireProgressRect.gameObject.SetActive(isTraining);
        this.candidateListRect.gameObject.SetActive(!isTraining);

        if(isTraining) {
            // Show training screen

            this.hireProgressSlider.maxValue = this.world.hireCandidates.workerTrainTime;
            this.hireProgressSlider.value = this.world.hireCandidates.remainingTrainingTime();
            
        } else {
            // Show hire screen

            if(!this.refreshedCandidates) {
                this.world.hireCandidates.refreshCanidates();

                this.refreshCandidateButtons();

                this.refreshedCandidates = true;
            }
        }
    }

    /// <summary>
    /// Updates the candidate buttons to reflect the possible new hire candidate.
    /// </summary>
    public void refreshCandidateButtons() {
        int candaditeCount = this.world.hireCandidates.candaditeCount;

        // Create buttons until there is enought
        while(this.candidateButtons.Count < candaditeCount) {
            CandidateButton btn = GameObject.Instantiate(this.candidateButtonPrefab, this.btnArea).GetComponent<CandidateButton>();
            int i = this.candidateButtons.Count;
            btn.GetComponentInChildren<Button>().onClick.AddListener(() => {
                btn.callback_hire(i);
            });
            this.candidateButtons.Add(btn);
        }

        for(int i = 0; i < this.candidateButtons.Count; i++) {
            CandidateButton btn = this.candidateButtons[i];

            Candidate c = this.world.hireCandidates.getCandidate(i);
            if(c == null) {
                btn.gameObject.SetActive(false);
            }
            else {
                btn.gameObject.SetActive(true);
                btn.setCandidate(c);
            }
        }

        for(int i = 0; i < candaditeCount; i++) {
            CandidateButton btn = this.candidateButtons[i];
            if(btn != null) {
                Candidate c = this.world.hireCandidates.getCandidate(i);
                if(c == null) {
                    btn.gameObject.SetActive(false);
                }
                else {
                    btn.gameObject.SetActive(true);
                    btn.setCandidate(c);
                }
            }
        }
    }
}
