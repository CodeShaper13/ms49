using System;

public class StatisticTime : StatisticFloat {

    public StatisticTime(string displayName, string saveName) : base(displayName, saveName) { }

    public override string ToString() {
        TimeSpan time = TimeSpan.FromSeconds(this.value);
        string s = time.ToString();
        if(s.Contains(".")) {
            s = s.Substring(0, s.LastIndexOf("."));
        }
        return this.displayName + ": " + s;
    }
}