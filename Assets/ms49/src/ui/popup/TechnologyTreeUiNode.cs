using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechnologyTreeUiNode : MonoBehaviour {

    [SerializeField]
    private TMP_Text _text = null;
    [SerializeField]
    private Image _img = null;
    [SerializeField]
    private Image _imgCostItem = null;
    [SerializeField]
    private TMP_Text _cost = null;

    public void Set(NodeTechTree node) {
        this._text.text = node.name;
        this._img.sprite = node.Image;
        // TODO cost
    }
}