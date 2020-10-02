using UnityEngine;
using fNbt;

public class Economy : MonoBehaviour, ISaveableState {

    [SerializeField]
    private IntVariable money = null;
    [SerializeField]
    private World world = null;

    public string tagName => "economy";

    private void Update() {
        if(Pause.isPaused()) {
            return;
        }

        // TODO
    }

    public void sellItem(Item item) {

    }

    public void readFromNbt(NbtCompound tag) {

    }

    public void writeToNbt(NbtCompound tag) {
    }
}
