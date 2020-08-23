using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ButtonAudioSource : MonoBehaviour {

    private AudioSource source;

    private void Awake() {
        this.source = this.GetComponent<AudioSource>();
    }

    public void play() {
        this.source.Play();
    }
}
