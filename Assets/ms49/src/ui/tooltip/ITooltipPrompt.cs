public interface ITooltipPrompt {

    string Text { get; }
    bool OverrideDelay { get; }
    float Delay { get; }
}