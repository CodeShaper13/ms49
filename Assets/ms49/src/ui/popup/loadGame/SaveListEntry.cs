using fNbt;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SaveListEntry : MonoBehaviour, IPointerDownHandler {

    [SerializeField]
    private TMP_Text saveNameText = null;
    [SerializeField]
    private PopupLoadGame _popupLoadGame = null;

    private Toggle toggle;

    public SaveFile saveFile { get; private set; }

    private void Awake() {
        this.toggle = this.GetComponent<Toggle>();
    }

    public void SetSave(SaveFile saveFile) {
        this.saveFile = saveFile;

        if(this.saveNameText != null) {
            this.saveNameText.text = string.Format(
                "{0} {1}",
                this.saveFile.saveName,
                this.saveFile.lastPlayTime.ToShortDateString());
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        this._popupLoadGame.selectedEntry = this;
        if(this.toggle.isOn) {
            // Player "Double clicked".
            PopupWindow.closeAll();

            NbtFile nbtFile = new NbtFile();
            nbtFile.LoadFromFile(this.saveFile.path);

            Main.instance.StartWorld(
                Path.GetFileNameWithoutExtension(this.saveFile.path),
                nbtFile.RootTag);
        }
    }
}
