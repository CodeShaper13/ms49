using UnityEngine;

public class PopupWindow : MonoBehaviour {

    /// <summary> The currently open popup window.  May be null. </summary>
    public static PopupWindow openPopup;

    [SerializeField]
    private bool _pauseGameWhenOpen = false;
    [SerializeField]
    private bool _blockInput = false;
    [SerializeField]
    private bool _closeableWithEscape = true;

    public bool pauseGameWhenOpen { get { return this._pauseGameWhenOpen; } }
    public bool blockInput { get { return this._blockInput; } }
    public bool closeableWithEscape { get { return this._closeableWithEscape; } }

    private void Awake() {
        this.initialize();
    }

    private void Update() {
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

    protected virtual void initialize() { }

    protected virtual void onUpdate() { }

    protected virtual void onOpen() { }

    protected virtual void onClose() { }
}
