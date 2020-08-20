using UnityEngine;
using UnityEngine.UI;

public class PopupDemo : PopupWindow {

    [SerializeField]
    [Min(0)]
    private int demoCost = 1;
    [SerializeField]
    private Sprite selectedIcon = null;
    [SerializeField]
    private Text text = null;
    [SerializeField]
    private Image btnImage = null;
    [SerializeField]
    private DemoHighlighter demoHighlighter = null;

    private Sprite originalSprite;

    public override void onAwake() {
        base.onAwake();

        this.originalSprite = this.btnImage.sprite;

        this.text.text = this.text.text.Replace("%", this.demoCost.ToString());
    }

    public override void onOpen() {
        base.onOpen();

        this.btnImage.sprite = this.selectedIcon;

        this.demoHighlighter.show();
    }

    public override void onClose() {
        base.onClose();

        this.btnImage.sprite = this.originalSprite;

        this.demoHighlighter.hide();
    }

    public int getDemoCost() {
        return this.demoCost;
    }
}
