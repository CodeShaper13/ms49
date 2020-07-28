using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupRoom : PopupWindow {

    [SerializeField]
    private GameObject btnPrefab = null;
    [SerializeField]
    private RectTransform area = null;
    [SerializeField]
    private Text roomDescriptionText = null;
    [SerializeField]
    private Room[] rooms = null;

    private Room selectedRoom;

    public override void onAwake() {
        base.onAwake();

        foreach(Room room in this.rooms) {
            if(room != null) {
                BtnRoomType btn = GameObject.Instantiate(this.btnPrefab).GetComponent<BtnRoomType>();
                btn.transform.SetParent(this.area, false);
                btn.setRoom(room);
            }
        }
    }

    public override void onOpen() {
        base.onOpen();

        this.setSelectedRoom(null);
    }

    public void setSelectedRoom(Room room) {
        this.selectedRoom = room;

        if(room != null) {
            this.roomDescriptionText.text = room.roomDescription;
        }
    }
}
