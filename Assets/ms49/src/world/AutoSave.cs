using UnityEngine;
using UnityEngine.UI;

public class AutoSave : MonoBehaviour {

    public static bool autoSave;

    [SerializeField, Tooltip("How often in seconds the game saves.")]
    private float _autoSaveInterval = 60 * 5;
    [SerializeField]
    private float _iconVisibleTime = 1f;
    [SerializeField]
    private Image _saveIcon = null;

    private World world;
    private float timer;

    private void Awake() {
        this._saveIcon.enabled = false;
    }

    private void OnEnable() {
        this.world = GameObject.FindObjectOfType<World>();
    }

    private void OnDisable() {
        this.StopAllCoroutines();
    }

    private void Update() {
        if(autoSave) {
            this.timer += Time.deltaTime;

            if(this.timer > this._autoSaveInterval) {
                this.world.SaveGame();
                this._saveIcon.enabled = true;

                this.timer = 0f;

                this.Invoke(nameof(this.Invoke_HideIcon), this._iconVisibleTime);
            }
        } else {
            this.timer = 0f;
        }
    }

    private void Invoke_HideIcon() {
        this._saveIcon.enabled = false;
    }
}
