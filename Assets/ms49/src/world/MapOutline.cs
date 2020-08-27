using UnityEngine;

public class MapOutline : MonoBehaviour {

    [SerializeField]
    private RectTransform rectTrans;

    public void setSize(int size) {
        this.rectTrans.sizeDelta = Vector2.one * size;
    }
}
