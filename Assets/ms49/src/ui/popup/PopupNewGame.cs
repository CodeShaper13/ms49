using UnityEngine;
using UnityEngine.UI;
using System;

public class PopupNewGame : PopupWindow {

    [SerializeField]
    public string defaultSaveName = "newworld";
    [SerializeField]
    private InputFieldSaveName fieldSaveName = null;
    [SerializeField]
    private InputField fieldSeed = null;
    [SerializeField]
    private Text mapSizeBtnText = null;
    [SerializeField]
    private Text creativeBtnText = null;
    [SerializeField]
    private Button buttonCreate = null;

    private int mapSize; // 0 = 32x32, 1 = 64x64, 2 = 128x128
    private bool creativeEnabled = false;

    private void OnValidate() {
        // Force to be lowercase.
        this.defaultSaveName = this.defaultSaveName.ToLower();
    }

    protected override void onOpen() {
        base.onOpen();
        
        // Reset fields
        this.mapSize = 1;
        this.creativeEnabled = false;
        this.fieldSaveName.text = this.defaultSaveName;

        // Update button labels
        this.updateMapSizeBtnText();
        this.updateCreativeBtnText();
    }

    protected override void onClose() {
        base.onClose();
    }

    protected override void onUpdate() {
        base.onUpdate();

        this.buttonCreate.interactable = this.fieldSaveName.isValidName;
    }

    public void callback_createWorld() {
        string s = this.fieldSeed.text;

        this.close();

        Main.instance.createWorld(
            this.fieldSaveName.text,
            new NewWorldSettings(
                s.Length > 0 ? s : DateTime.Now.ToBinary().ToString(),
                (EnumMapSize)this.mapSize,
                this.creativeEnabled));
    }

    public void callback_mapSize() {
        this.mapSize++;
        if(this.mapSize > 2) {
            this.mapSize = 0;
        }

        this.updateMapSizeBtnText();
    }

    public void callback_creativeBtn() {
        this.creativeEnabled = !this.creativeEnabled;

        this.updateCreativeBtnText();
    }

    private void updateMapSizeBtnText() {
        this.mapSizeBtnText.text = this.mapSize == 0 ? "small" : this.mapSize == 1 ? "medium" : "large";
    }

    private void updateCreativeBtnText() {
        this.creativeBtnText.text = this.creativeEnabled ? "on" : "off";
    }
}
