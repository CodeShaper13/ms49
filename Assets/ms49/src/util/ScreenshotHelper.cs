using System;
using System.IO;
using UnityEngine;

public class ScreenshotHelper : MonoBehaviour {

    /// <summary>
    /// The default directory to save screenshots to.
    /// </summary>
    [Tooltip("The location that screenshots are saved to.")]
    public string defaultDirectory = "screenshots/";
    [Tooltip("What key causes a screenshot to be captured.")]
    public KeyCode captureKey = KeyCode.F2;

    private void Update() {
        if(Input.GetKeyDown(this.captureKey)) {
            this.captureScreenshot();
        }
    }

    private void OnValidate() {
        if(!this.defaultDirectory.EndsWith("/")) {
            this.defaultDirectory += "/";
        }
    }

    /// <summary>
    /// Takes a screenshot.  When no path is passes, the default screenshot directory is used with the current time as the name.
    /// </summary>
    public void captureScreenshot(string screenshotPath = null) {
        if(screenshotPath == null) {
            string time = DateTime.Now.ToString();
            time = time.Replace('/', '-').Replace(' ', '_').Replace(':', '.').Substring(0, time.Length - 3);

            screenshotPath = this.defaultDirectory + time + ".png";
        }

        if(!Directory.Exists(this.defaultDirectory)) {
            Directory.CreateDirectory(this.defaultDirectory);
        }

        ScreenCapture.CaptureScreenshot(screenshotPath);
        Debug.Log("Saved screenshot to " + screenshotPath);
    }
}