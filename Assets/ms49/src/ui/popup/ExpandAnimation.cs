using UnityEngine;

/// <summary>
/// Causes the transfrom to expand to it's original size over time.
/// </summary>
[DisallowMultipleComponent]
public class ExpandAnimation : MonoBehaviour {

    private const float EXPAND_TIME = 14f;

    private float timeOpen;
    private Vector3 originalScale;

    private void OnEnable() {
        this.originalScale = Vector3.one; // this.transform.localScale;
        this.transform.localScale = Vector3.zero;
        this.timeOpen = 0f;
    }

    private void Update() {
        float f = Mathf.Clamp01(this.timeOpen * EXPAND_TIME);
        this.transform.localScale = this.originalScale * f;

        this.timeOpen += Time.fixedUnscaledDeltaTime;
    }
}
