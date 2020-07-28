using UnityEngine;
using UnityEngine.UI;

public class BtnRoomType : MonoBehaviour {

    [SerializeField]
    private Image highlightEffect;

    private PopupRoom popupRoom;
    private Room room;

    private void Start() {
        this.popupRoom = this.GetComponentInParent<PopupRoom>();
    }

    public void setRoom(Room room) {
        this.room = room;
    }

    public void callback() {
        this.popupRoom.setSelectedRoom(room);
    }
}
