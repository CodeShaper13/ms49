using UnityEngine;

public class FeaturePipes : FeatureBase {

    [SerializeField]
    private CellData _pipe = null;

    public override void generate(System.Random rnd, LayerData layerData, MapAccessor accessor) {
        if(accessor.depth == 0) {
            int pipeCount = 3;

            Rotation pipeState = Rotation.UP;
            for(int i = 0; i < pipeCount; i++) {
                int x = Random.Range(0, accessor.size);

                Position pipePos = new Position(x, accessor.size - 1, 0);

                if(accessor.getCell(pipePos.x, pipePos.y) == this._pipe) {
                    continue; // don't start a pipe on top of another one
                }

                bool direction = true;
                int straightDistance = 0;
                int SAFETY = 0;
                while(true) {
                    SAFETY++;

                    straightDistance++;

                    accessor.setCell(pipePos.x, pipePos.y, this._pipe);
                    accessor.setRot(pipePos.x, pipePos.y, pipeState);

                    if(straightDistance > 10 || (straightDistance > 4 && Random.value < 0.25)) {
                        direction = !direction;
                        straightDistance = 0;
                    }

                    if(direction) {
                        pipePos = pipePos.Add(0, -1);
                    } else {
                        pipePos = pipePos.Add(x <= (accessor.size / 2) ? -1 : 1, 0);
                    }

                    if(!accessor.inBounds(pipePos.x, pipePos.y)) {
                        break;
                    }

                    if(SAFETY > 500) {
                        print("safety break");
                        break;
                    }
                }

                pipeState = pipeState.clockwise();
            }
        }
    }
}
