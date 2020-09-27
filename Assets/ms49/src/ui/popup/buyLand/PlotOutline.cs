using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlotOutline : MonoBehaviour {

    [SerializeField]
    private Text text = null;
    [SerializeField]
    private Button button = null;
    [SerializeField]
    private RectTransform outline = null;

    public Plot plot { get; set; }

    public void setPlot(Plot p) {
        this.plot = p;

        this.outline.position = (Vector2)this.plot.rect.min;
        this.outline.sizeDelta = this.plot.rect.size;

        this.text.text = "$" + p.cost;
    }

    public void setClickCallback(UnityAction action) {
        this.button.onClick.AddListener(action);
    }
}
