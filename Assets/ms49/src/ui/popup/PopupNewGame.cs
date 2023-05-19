using UnityEngine;
using UnityEngine.UI;
using System;
using NaughtyAttributes;
using TMPro;

public class PopupNewGame : PopupWindow {

    [SerializeField]
    public string defaultSaveName = "newworld";
    [SerializeField, Required]
    private InputFieldSaveName fieldSaveName = null;
    [SerializeField, Required]
    private TMP_InputField _fieldSeed = null;
    [SerializeField, Required]
    private Toggle _toggleCreative = null;
    [SerializeField]
    private Button _buttonCreate = null;

    private int mapSize; // 0 = 32x32, 1 = 64x64, 2 = 128x128

    protected override void onOpen() {
        base.onOpen();
        
        // Reset fields
        this.mapSize = 1;
        this._toggleCreative.isOn = false;
        this.fieldSaveName.text = this.defaultSaveName;
    }

    protected override void onUpdate() {
        base.onUpdate();

        this._buttonCreate.interactable = this.fieldSaveName.isValidName;
    }

    public void Callback_CreateWorld() {
        string s = this._fieldSeed.text;

        Main.instance.StartWorld(
            this.fieldSaveName.text,
            new NewWorldSettings(
                s.Length > 0 ? s : DateTime.Now.ToBinary().ToString(),
                (EnumMapSize)this.mapSize,
                this._toggleCreative.isOn));
    }
}
