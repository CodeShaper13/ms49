using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfimDialog : PopupWindow {

    private readonly string YES = "Yes";
    private readonly string NO = "No";

    [SerializeField, Required]
    private TMP_Text _mainMsg = null;
    [SerializeField, Required]
    private TMP_Text _secondaryMsg = null;
    [SerializeField, Required]
    private TMP_Text _btnYesText = null;
    [SerializeField, Required]
    private TMP_Text _btnNoText = null;
    [SerializeField, Required]
    private Button _btnNo = null;

    public Action yesCallback;
    public Action noCallback;
    public string headerText {
        get => this._mainMsg.text;
        set {
            this._mainMsg.text = value;
        }
    }
    public string messageText {
        get => this._secondaryMsg.text;
        set {
            this._secondaryMsg.text = value;
        }
    }
    public string yesBtnText {
        get => this._btnYesText.text;
        set {
            this._btnYesText.text = value;
        }
    }
    public string noBtnText {
        get => this._btnNoText.text;
        set {
            this._btnNoText.text = value;
        }
    }
    public bool noBtnVisable {
        get => this._btnNo.gameObject.activeSelf;
        set {
            this._btnNo.gameObject.SetActive(value);
        }
    }

    private void Awake() {
        // Assign the fields their default values, if nothing else
        // has jumped the gun and already given them a value.
        if(this.yesCallback == null) {
            this.yesCallback = this.Close;
        }
        if(this.noCallback == null) {
            this.noCallback = this.Close;
        }
    }

    protected override void onClose() {
        base.onClose();

        this._mainMsg.text = string.Empty;
        this._secondaryMsg.text = string.Empty;

        this.yesCallback = this.Close;
        this.noCallback = this.Close;

        this._btnYesText.text = YES;
        this._btnNoText.text = NO;

        this.noBtnVisable = true;
    }

    public void Callback_Yes() {
        if(this.yesCallback != null) {
            this.yesCallback();
        }
        else {
            Debug.LogWarning(
                "PopupConfirmDialog has no callback set for the \"Yes\" button",
                this.gameObject);
        }
    }

    public void Callback_No() {
        if(this.noCallback != null) {
            this.noCallback();
        }
        else {
            Debug.LogWarning(
                "PopupConfirmDialog has no callback set for the \"No\" button",
                this.gameObject);
        }
    }
}