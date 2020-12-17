public class RegisteredStat {

    public readonly EnumStatisticCategory category;
    public readonly IStatistic stat;
    
    public RegisteredStat(IStatistic stat, EnumStatisticCategory category) {
        this.stat = stat;
        this.category = category;
    }
}
