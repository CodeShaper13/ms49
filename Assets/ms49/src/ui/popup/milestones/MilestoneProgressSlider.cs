using UnityEngine;
using UnityEngine.UI;

public class MilestoneProgressSlider : MonoBehaviour {

    [SerializeField]
    private Text text = null;
    [SerializeField]
    private Slider slider = null;
    [SerializeField]
    private TooltipPrompt tooltip = null;

    private string originalText;
    private MilestoneRequirerment requirement;

    private void Awake() {
        this.originalText = this.text.text;
    }

    public void setRequirement(MilestoneRequirerment requirement, World world) {
        this.requirement = requirement;

        int progress = this.requirement.getProgress(world);

        // Set text
        if(this.text != null) {
            this.text.text = string.Format(
                this.originalText,
                this.requirement.title,
                Mathf.Min(progress, this.requirement.targetAmount),
                this.requirement.targetAmount);
        }

        // Set slider
        if(this.slider != null) {
            this.slider.maxValue = this.requirement.targetAmount;
            this.slider.value = progress;
        }

        // Set tooltip
        if(this.tooltip != null) {
            this.tooltip.text = string.Format(this.requirement.tooltip, this.requirement.targetAmount);
        }
    }
}
