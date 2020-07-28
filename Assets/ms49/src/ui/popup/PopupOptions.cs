using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupOptions : PopupWindow {

    public void callback_fullscreenToggle(Toggle toggle) {
        Screen.fullScreen = toggle.isOn;
    }

    public void callback_fxSlider(Slider slider) {
        AudioListener.volume = slider.value / 100f;
    }

    public void callback_musicSlider(Slider slider) {

    }
}
