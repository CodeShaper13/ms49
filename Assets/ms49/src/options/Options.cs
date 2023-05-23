using UnityEngine;
using System.Collections.Generic;

public class Options : MonoBehaviour {

    public List<IOption> allOptions { get; private set; }

    private void Awake() {
        this.allOptions = new List<IOption>();

        // Create the Options.
        this.allOptions.Add(new OptionToggle(
            "Fullscreen",
            "isFullscreen",
            null,
            () => { return Screen.fullScreen; },
            (bool value) => { Screen.fullScreen = value; }
        ));
        this.allOptions.Add(new OptionToggle(
            "Vertical Sync",
            "vSync",
            "Limits the framerate to the monitor's refresh rate.  May prevent screen tearing",
            () => { return QualitySettings.vSyncCount == 1; },
            (bool value) => { QualitySettings.vSyncCount = value ? 1 : 0; }
        ));

        this.allOptions.Add(new OptionSlider(
            "Game Volume",
            "sfxVolume",
            null,
            0f,
            1f,
            false,
            () => { return AudioListener.volume; },
            (float value) => { AudioListener.volume = value; }
        ));

        this.allOptions.Add(new OptionToggle(
            "Auto Save Game",
            "autoSave",
            "If enabled, the game will auto save every 5 minutes",
            () => { return AutoSave.autoSave; },
            (bool value) => { AutoSave.autoSave = value; }
        ));


        // Read all of the options from the Player prefs.
        foreach(IOption option in this.allOptions) {
            option.read();
            option.applyValue();
        }
    }

    /// <summary>
    /// Saves all of the options, writing their values to disk and applying them to the game.
    /// </summary>
    public void saveOptions() {
        foreach(IOption option in this.allOptions) {
            option.write();
            option.applyValue();
        }
    }
}
