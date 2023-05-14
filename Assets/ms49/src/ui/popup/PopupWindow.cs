using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindow : MonoBehaviour {

    public static List<PopupWindow> openPopups = new List<PopupWindow>();

    [SerializeField]
    private bool _pauseGameWhenOpen = false;
    [SerializeField, Tooltip("If checked, normal camera movement, panning, zooming, ect., is blocked")]
    private bool _blockInput = false;
    [SerializeField]
    private bool _closeableWithEscape = true;

    public bool PauseGameWhenOpen => this._pauseGameWhenOpen;
    public bool BlockInput => this._blockInput;
    public bool CloseableWithEscape => this._closeableWithEscape;
    public bool IsOpen => PopupWindow.openPopups.Contains(this);

    /// <summary>
    /// Returns true if at least one of the open Popups is blocking input
    /// </summary>
    public static bool blockingInput() {
        foreach(PopupWindow popup in openPopups) {
            if(popup.BlockInput) {
                return true;
            }
        }
        return false;
    }

    public static void closeAll() {
        for(int i = openPopups.Count - 1; i >= 0; i--) {
            openPopups[i].close();
        }
    }

    /// <summary>
    /// Returns the number of Popup Windows open.
    /// </summary>
    /// <returns></returns>
    public static int getPopupsOpen() {
        return PopupWindow.openPopups.Count;
    }

    private void Awake() {
        // Disable the imediate children in case the user forgot.
        //this.setChildState(false);
    }

    private void Update() {
        this.onUpdate();
    }

    public void open() {
        this.open(true);
    }

    public void openAdditive() {
        this.open(false);
    }

    private void open(bool closeAllOpen) {
        if(closeAllOpen) {
            for(int i = openPopups.Count - 1; i >= 0; i--) {
                openPopups[i].close();
            }
        }

        this.gameObject.SetActive(true);
        openPopups.Add(this);

        // Pause the game if the popup requires it.
        if(this.PauseGameWhenOpen) {
            Pause.PauseGame();
        }

        this.onOpen();
    }

    public void close() {
        this.onClose();

        openPopups.Remove(this);
        this.gameObject.SetActive(false);

        // Unpause the game if none of the open windows require the game to be paused.
        bool unPause = true;
        foreach(PopupWindow popup in openPopups) {
            if(popup.PauseGameWhenOpen) {
                unPause = false;
                break;
            }
        }

        if(unPause) {
            Pause.UnpauseGame();
        }
    }

    protected virtual void onUpdate() { }

    protected virtual void onOpen() { }

    protected virtual void onClose() { }

    private void setChildState(bool active) {
        foreach(Transform child in this.transform) {
            print("a");
            child.gameObject.SetActive(active);
        }
    }

#if UNITY_EDITOR
    [Button("Open")]
    private void Editor_Open() {
        if(Application.isPlaying) {
            this.open();
        }
        else {
            print("!");
            this.setChildState(true);
        }
    }

    [Button("Close")]
    private void Editor_Close() {
        if(Application.isPlaying) {
            this.close();
        }
        else {
            this.setChildState(false);
        }
    }
#endif
}
