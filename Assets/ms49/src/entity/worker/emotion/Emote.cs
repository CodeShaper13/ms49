public class Emote {

    public string iconName { get; private set; }
    public float timeVisible { get; private set; }
    public bool isPriority { get; private set; }
    public string tooltip { get; private set; }

    public Emote(string iconName, float timeVisible) {
        this.iconName = iconName;
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
