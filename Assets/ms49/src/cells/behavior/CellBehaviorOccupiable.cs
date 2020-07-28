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
}
