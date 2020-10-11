using UnityEngine;
using fNbt;

public class Economy : MonoBehaviour, ISaveableState {

    [SerializeField]
    private World world = null;

    private float[] itemValueMultiplyers;
    private MinedItemRegistry reg;

    public string tagName => "economy";

    private void Awake() {
        this.reg = Main.instance.itemRegistry;

        this.itemValueMultiplyers = new float[this.reg.getRegistrySize()];
        for(int i = 0; i < this.itemValueMultiplyers.Length; i++) {
            this.itemValueMultiplyers[i] = 1f;
        }
    }

    private void Update() {
        if(Pause.isPaused()) {
            return;
        }

        // TODO
    }

    /// <summary>
    /// Returns the value of the passed item adjusted based on the economy.
    /// </summary>
    public int getItemValue(Item item) {
        if(item.includeInEconemy) {
            return Mathf.RoundToInt(item.moneyValue * this.itemValueMultiplyers[this.reg.getIdOfElement(item)]);
        } else {
            return item.moneyValue;
        }
    }

    /// <summary>
    /// Sells the passed item for it's current value.
    /// </summary>
    public void sellItem(Item item) {
        if(item != null) {
            this.world.money.value += this.getItemValue(item);
        }
    }

    public void readFromNbt(NbtCompound tag) {

    }

    public void writeToNbt(NbtCompound tag) {

    }
}
