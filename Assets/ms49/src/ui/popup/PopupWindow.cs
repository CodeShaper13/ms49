using UnityEngine;

public class PopupWindow : MonoBehaviour {

    /// <summary> The currently open popup window.  May be null. </summary>
    public static PopupWindow openPopup;

    [SerializeField]
    private Transform frameTransform = null;
    [SerializeField]
    private bool _pauseGameWhenOpen = false;
    [SerializeField]
    private bool _blockInput = false;

    [Space]

    private float timeOpen;

    public bool pauseGameWhenOpen { get { return this._pauseGameWhenOpen; } }
    public bool blockInput { get { return this._blockInput; } }

    private void Awake() {
        this.initialize();
    }

    private void OnEnable() {
        this.frameTransform.localScale = Vector3.zero;
    }

    private void Update() {
        float f = Mathf.Clamp01(this.timeOpen * 14);
        this.frameTransform.localScale = Vector3.one * f;

        this.timeOpen += Time.fixedUnscaledDeltaTime;

        this.onUpdate();
    }

    public static bool blockingInput() {
        return PopupWindow.openPopup != null && PopupWindow.openPopup.blockInput;
    }

    public void open() {
        if(PopupWindow.openPopup != null) {
            PopupWindow.openPopup.close();
        }

        this.gameObject.SetActive(true);
        PopupWindow.openPopup = this;

        // Pause the game if the popup requires it.
        if(this.pauseGameWhenOpen) {
            Pause.pause();
        }

        this.onOpen();

        this.timeOpen = 0f;
    }

    public void close() {
        this.onClose();

        // Unpause the game if the open popup requires the game to be paused.
        if(PopupWindow.openPopup != null) {
            if(PopupWindow.openPopup.pauseGameWhenOpen) {
                Pause.unPause();
            }
        }

        PopupWindow.openPopup = null;
        this.gameObject.SetActive(false);
    }

    public bool isOpen() {
        return PopupWindow.openPopup == this;
    }

    public virtual void initialize() { }

    public virtual void onUpdate() { }

    public virtual void onOpen() { }

    public virtual void onClose() { }
}
