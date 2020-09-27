using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;

public class PopupNewGame : PopupWindow {

    [SerializeField]
    private string regexWorldName = @"[^a-zA-Z0-9_!]";
    [SerializeField]
    private InputField fieldSaveName = null;
    [SerializeField]
    private InputField fieldSeed = null;
    [SerializeField]
    private Text mapSizeBtnText = null;
    [SerializeField]
    private Text creativeBtnText = null;
    [SerializeField]
    private Button buttonCreate = null;
    [SerializeField]
    private Image invalidNameIcon = null;

    private int mapSize; // 0 = 32x32, 1 = 64x64, 2 = 128x128
    private bool creativeEnabled = false;
    private List<string> cachedSaveFileNames;

    protected override void onOpen() {
        base.onOpen();

        this.cachedSaveFileNames = Main.instance.getAllSaves();


        // Update invalid save name icon
        this.invalidNameIcon.enabled = false;
        
        // Reset fields
        this.mapSize = 1;
        this.creativeEnabled = false;
        this.fieldSaveName.text = "new world";

        // Update button labels
        this.updateMapSizeBtnText();
        this.updateCreativeBtnText();
    }

    protected override void onClose() {
        base.onClose();
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

    public void callback_characterChange() {
        // Remove invalid characters
        this.fieldSaveName.text = Regex.Replace(this.fieldSaveName.text, this.regexWorldName, "");

        // Update the create button interactability
        bool isValidName = !string.IsNullOrEmpty(this.fieldSaveName.text);
        foreach(string saveName in this.cachedSaveFileNames) {
            if(Path.GetFileNameWithoutExtension(saveName) == this.fieldSaveName.text) {
                isValidName = false;
                break;
            }
        }
        this.buttonCreate.interactable = isValidName;
        this.invalidNameIcon.enabled = !isValidName;
    }

    private void updateMapSizeBtnText() {
        this.mapSizeBtnText.text = this.mapSize == 0 ? "small" : this.mapSize == 1 ? "medium" : "large";
    }

    private void updateCreativeBtnText() {
        this.creativeBtnText.text = this.creativeEnabled ? "on" : "off";
    }
}
