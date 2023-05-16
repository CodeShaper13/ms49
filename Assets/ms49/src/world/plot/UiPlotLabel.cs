using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiPlotLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField]
    private TMP_Text _textPrice;

    public void OnPointerEnter(PointerEventData eventData) {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData) {
        throw new System.NotImplementedException();
    }

    public void Callback_OnClick() {

    }
}