using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindow : MonoBehaviour {

    /// <summary> The currently open popup window.  May be null. </summary>
    //public static PopupWindow openPopup;

    public static List<PopupWindow> openPopups = new List<PopupWindow>();

    [SerializeField]
    private bool _pauseGameWhenOpen = false;
    [SerializeField]
    private bool _blockInput = false;
    [SerializeField]
    private bool _closeableWithEscape = true;

    private bool isPopupOpen;

    public bool pauseGameWhenOpen { get { return this._pauseGameWhenOpen; } }
    public bool blockInput { get { return this._blockInput; } }
    public bool closeableWithEscape { get { return this._closeableWithEscape; } }
    public bool isOpen => PopupWindow.openPopups.Contains(this);

    /// <summary>
    /// Returns true if at least one of the open Popups is blocking input
    /// </summary>
    public static bool blockingInput() {
        foreach(PopupWindow popup in openPopups) {
            if(popup.blockInput) {
                print(popup.name + " is blocking input");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the number of Popup Windows open.
    /// </summary>
    /// <returns></returns>
    public static int getPopupsOpen() {
        return openPopups.Count;
    }

    private void Awake() {
        this.initialize();
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
        if(this.pauseGameWhenOpen) {
            Pause.pause();
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
            if(popup.pauseGameWhenOpen) {
                unPause = false;
                break;
            }
        }

        if(unPause) {
            Pause.unPause();
        }
    }

    protected virtual void initialize() { }

    protected virtual void onUpdate() { }

    protected virtual void onOpen() { }

    protected virtual void onClose() { }
}
