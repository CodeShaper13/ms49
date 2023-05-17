using System.Collections.Generic;
using UnityEngine;
public class PopupBuyLand : PopupWorldReference {

    [SerializeField]
    private GameObject labelPrefab = null;
    [SerializeField]
    private Transform labelHolder = null;
    [SerializeField]
    private PopupBuyLandConfirm popup = null;

    private List<PlotOutline> labels;

    private void Awake() {
        this.labels = new List<PlotOutline>();
    }

    protected override void onUpdate() {
        base.onUpdate();

        foreach(PlotOutline outlineLabel in this.labels) {
            // Hide the element if the plot is owned, or all
            // neightbors are unowned.

            bool isAdjacentOwned = false;
            Vector2Int pcp = outlineLabel.plot.plotCoordPos;
            foreach(Rotation r in Rotation.ALL) {
                int x = pcp.x + r.vector.x;
                int y = pcp.y + r.vector.y;
                Plot plot = this.world.plotManager.GetPlotFromPlotCoords(x, y);
                if(plot != null && plot.isOwned) {
                    isAdjacentOwned = true;
                    break;
                }
            }

            outlineLabel.gameObject.SetActive(isAdjacentOwned && !outlineLabel.plot.isOwned);
        }
    }

    protected override void onOpen() {
        base.onOpen();

        // Create labels for all of the plots
        foreach(Plot p in this.world.plotManager.plots) {
            if(p != null) { // TODO is adjacent to owned
                PlotOutline outlineLabel = GameObject.Instantiate(this.labelPrefab, this.labelHolder).GetComponent<PlotOutline>();
                outlineLabel.gameObject.SetActive(true);
                outlineLabel.SetPlot(p);

                this.labels.Add(outlineLabel);
            }
        }

        CameraController.instance.setZoom(int.MinValue); // Fully zoomed out
    }

    public void Callback_BuyLand(PlotOutline outline) {
        if(this.popup != null) {
            this.popup.openAdditive();
            this.popup.setPlot(outline.plot);
        }
    }

    protected override void onClose() {
        base.onClose();

        // Destroy all of the label objects.
        foreach(PlotOutline label in this.labels) {
            GameObject.Destroy(label.gameObject);
        }
        this.labels.Clear();
    }
}
