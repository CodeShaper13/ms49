using UnityEngine;
using UnityEngine.UI;

public class HeatmapLegendGenerator : MonoBehaviour {

    [SerializeField]
    private ColorVariable colorLow = null;
    [SerializeField]
    private ColorVariable colorMedium = null;
    [SerializeField]
    private ColorVariable colorHigh = null;
    [SerializeField]
    private RawImage img = null;

    [SerializeField, Min(1)]
    private int segments = 4;

    private void Awake() {
        int totalSegments = (this.segments * 2) + 1;
        Texture2D text = new Texture2D(totalSegments, 1);
        float fraction = 1f / (this.segments);
        for(int i = 0; i < totalSegments; i++) {
            Color color;

            if(i < this.segments) {
                color = Color.Lerp(this.colorLow, this.colorMedium, fraction * i);
            } else if(i == segments) {
                color = this.colorMedium;
            } else {
                color = Color.Lerp(this.colorMedium, this.colorHigh, fraction * (i - segments));
            }

            text.SetPixel(i, 0, color);
        }

        text.filterMode = FilterMode.Point;

        text.Apply();

        this.img.texture = text;
    }
}
