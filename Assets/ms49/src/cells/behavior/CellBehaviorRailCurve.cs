using fNbt;
using static CellDataRail;

public class CellBehaviorRailCurve : CellBehavior, IHasData, ILeverReciever {

    private bool isFlipped;

    public void onLeverFlip(CellBehaviorLever lever) {
        Rotation r = this.rotation;

        for(int i = 0; i < 3; i++) {
            r = r.clockwise();

            // Make sure both sides of the curve point towards a rail
            if(this.isValidRail(r) && this.isValidRail(r.clockwise())) {
                this.state.rotation = r;
                this.dirty();
                break;
            }
        }
    }

    public void readFromNbt(NbtCompound tag) {
        this.isFlipped = tag.getBool("isSwitchCurved");
    }

    public void writeToNbt(NbtCompound tag) {
        tag.setTag("isSwitchCurved", this.isFlipped);
    }

    private bool isValidRail(Rotation r) {
        CellState state = this.world.getCellState(this.pos + r);
        if(state.data is CellDataRail) {
            return true; 

            /*
            Rotation facingRot = state.rotation;
            EnumRailMoveType railType = ((CellDataRail)state.data).moveType;

            if(railType == EnumRailMoveType.STRAIGHT) {
                return r.axis == facingRot.axis;
            }
            else if(railType == EnumRailMoveType.CROSSING) {
                return true;
            }
            else if(railType == EnumRailMoveType.CURVE) {
                return r.axis == 
            }
            else if(railType == EnumRailMoveType.STOPPER) {
                return r == facingRot.opposite();
            }
            */
        }

        return false;
    }
}
