public class CellBehaviorDresser : CellBehavior {

    public Position owningBedPos { get; set; }

    public bool isClaimed => this.owningBedPos != null;
}
