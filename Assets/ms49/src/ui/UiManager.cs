﻿using UnityEngine;

public class UiManager : MonoBehaviour {

    public static UiManager singleton;

    [SerializeField]
    private KeyCode toggleKey = KeyCode.F1;
    [SerializeField]
    private Transform uiHolder = null;

    /// <summary> The UI that is currenlty open.  May be null. </summary>
    private UIBase currentUI;

    private bool visibleWhenOpened;

    private void Awake() {
        UiManager.singleton = this;

#if UNITY_EDITOR
        // Hide the UI objects if one got left open by accident.
        foreach(UIBase ui in this.GetComponentsInChildren<UIBase>()) {
            ui.gameObject.SetActive(false);
        }
#endif
    }

    private void Update() {
        // Close the current popup if escape is pressed.
        if(Input.GetButtonDown("Cancel")) {
            if(PopupWindow.getPopupsOpen() == 0) {
                // Open the Pause screen if there is a world loaded.
                if(Main.instance.isPlaying()) {
                    PopupPause popup = Main.instance.findPopup<PopupPause>();
                    if(popup != null) {
                        popup.open();
                    }
                }
            }
            else {
                // Close all of the Popup windows that are open
                for(int i = PopupWindow.openPopups.Count - 1; i >= 0; i--) {
                    PopupWindow popup = PopupWindow.openPopups[i];
                    if(popup.closeableWithEscape) {
                        popup.close();
                    }
                }
            }
        }

        // Hide/show the ui if the toggle key is pressed.
        if(Input.GetKeyDown(this.toggleKey)) {
            this.uiHolder.gameObject.SetActive(!this.uiHolder.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// Closes the current UI and calls the UiBase#onHide() method for cleanup.
    /// </summary>
    public void closeCurrent() {
        if(this.currentUI != null) {
            this.currentUI.gameObject.SetActive(false);
            this.currentUI.onClose();
            this.currentUI = null;
        }

        // Restore the cursor to how it was when the ui was opened.
        if(this.visibleWhenOpened) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Opens the passed UI and then returns it.
    /// </summary>
    public UIBase openUI(UIBase ui) {
        this.closeCurrent();

        this.visibleWhenOpened = Cursor.visible;

        // Unlock and show the cursor if it's locked.
        if(!Cursor.visible) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        this.currentUI = ui;
        ui.gameObject.SetActive(true);
        ui.onShow();
        return ui;
    }

    /// <summary>
    /// May be null.
    /// </summary>
    public UIBase getCurrentUI() {
        return this.currentUI;
    }
}
