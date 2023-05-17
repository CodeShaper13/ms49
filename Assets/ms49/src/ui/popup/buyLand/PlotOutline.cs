using TMPro;
using UnityEngine;

public class PlotOutline : MonoBehaviour {

    [SerializeField]
    private TMP_Text text = null;
    [SerializeField]
    private RectTransform outline = null;

    public Plot plot { get; private set; }

    public void SetPlot(Plot p) {
        this.plot = p;

        this.outline.position = (Vector2)this.plot.rect.min;
        this.outline.sizeDelta = this.plot.rect.size;

        this.text.text = string.Format("${0}", p.cost);
    }
}
