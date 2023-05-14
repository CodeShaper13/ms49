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

    public override void getDebugText(StringBuilder sb, string indent) {
        base.getDebugText(sb, indent);

        sb.Append(indent + "Occupent: " + (this.isOccupied() ? this.occupant.name : "NONE"));
    }
}
