using UnityEngine;
using UnityEngine.UI;

public class PopupDemo : PopupWindow {

    [SerializeField]
    [Min(0)]
    private int demoCost = 1;
    [SerializeField]
    private Text text = null;
    [SerializeField]
    private DemoHighlighter demoHighlighter = null;

    protected override void initialize() {
        base.initialize();

        this.text.text = this.text.text.Replace("%", this.demoCost.ToString());
    }

    protected override void onOpen() {
        base.onOpen();

        this.demoHighlighter.show();
    }

    protected override void onClose() {
        base.onClose();

        this.demoHighlighter.hide();
    }

    public int getDemoCost() {
        return this.demoCost;
    }
}
