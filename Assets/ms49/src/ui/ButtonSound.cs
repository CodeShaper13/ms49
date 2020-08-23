using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour {

    private ButtonAudioSource btnAudioSource;

    private void Awake() {
        this.btnAudioSource = GameObject.FindObjectOfType<ButtonAudioSource>();

        if(this.btnAudioSource != null) {
            Button btn = this.GetComponent<Button>();
            btn.onClick.AddListener(() => {
                this.btnAudioSource.play();
            });
        } else {
            Debug.LogWarning("Could not find ButtonAudioSource script.  Button sounds will be disabled.");
        }
    }
}
