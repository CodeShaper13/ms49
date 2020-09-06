using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Options : MonoBehaviour {

    [SerializeField]
    private GameObject sliderPrefab;
    [SerializeField]
    private GameObject togglePrefab;

    public List<IOption> allOptions { get; private set; }

    private void Awake() {
        this.allOptions = new List<IOption>();

        // Create the Options.
        this.allOptions.Add(new OptionToggle(
            "Fullscreen",
            "isFullscreen",
            () => { return Screen.fullScreen; },
            (bool value) => { Screen.fullScreen = value; }));

        this.allOptions.Add(new OptionSlider(
            "game volume",
            "sfxVolume",
            0f,
            1f,
            false,
            () => { return AudioListener.volume; },
            (float value) => { AudioListener.volume = value; }));


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
