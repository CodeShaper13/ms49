using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour {

    [SerializeField]
    private TMP_Text _textDate = null;
    [SerializeField]
    private Image _imgPayPeriodProgressBar = null;
    [SerializeField, Required]
    private TooltipPrompt _tooltipPayPeriodBar = null;

    private GameTime gameTime;
    private World world;

    private void OnEnable() {
        this.world = Main.instance.activeWorld;
        this.gameTime = this.world.time;
    }

    private void LateUpdate() {
        if(this._textDate != null) {
            this._textDate.text = this.gameTime.GetFormattedTimeExact(); // this.gameTime.GetWorldTime().ToString("MM/dd/yyyy");
        }

        if(this._imgPayPeriodProgressBar != null) {
            int maxValue = this.world.payroll.payRate;
            float value = (float)(this.gameTime.Time - this.world.payroll.lastPayTime);

            this._imgPayPeriodProgressBar.fillAmount = value / maxValue;
            this._tooltipPayPeriodBar.text = string.Format("{0} seconds until payday", (int)(this.world.payroll.payRate - value));
        }
    }
}