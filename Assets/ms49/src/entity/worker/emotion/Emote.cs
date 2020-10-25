using UnityEngine;
using UnityEngine.UI;

public class Emote {

    public Sprite sprite { get; private set; }
    public float timeVisible { get; private set; }
    public bool isPriority { get; private set; }
    public string tooltip { get; private set; }

    public Emote(Sprite sprite, float timeVisible) {
        this.sprite = sprite;
        this.timeVisible = timeVisible;
    }

    public Emote setTooltip(string tooltip) {
        this.tooltip = tooltip;
        return this;
    }

    public Emote setPriority() {
        this.isPriority = true;
        return this;
    }
}
