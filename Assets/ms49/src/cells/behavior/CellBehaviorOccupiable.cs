using System.Text;

public abstract class CellBehaviorOccupiable : CellBehavior {

    private EntityWorker occupant;

    public bool isOccupied() {
        return this.occupant != null;
    }

    public EntityWorker getOccupant() {
        return this.occupant;
    }

    public void setOccupant(EntityWorker worker) {
        this.occupant = worker;
    }

    public override void GetDebugText(StringBuilder sb, string indent) {
        base.GetDebugText(sb, indent);

        sb.Append(indent + "Occupent: " + (this.isOccupied() ? this.occupant.name : "NONE"));
    }
}
