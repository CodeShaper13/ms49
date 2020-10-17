using UnityEngine;
using fNbt;
using System.Collections.Generic;

public class Economy : MonoBehaviour, ISaveableState {

    [SerializeField]
    private World _world = null;
    [SerializeField, MinMaxSlider(0, 600), Tooltip("In seconds")]
    private Vector2 _flatMarketTime = new Vector2(1, 2);
    [SerializeField, MinMaxSlider(0, 600), Tooltip("In seconds")]
    private Vector2 _curveLengthRange = new Vector2(1, 2);
    [SerializeField]
    private AnimationCurve _curve = null;
    [SerializeField]
    public float _curvePriceMultiplyer = 0.5f; // TODO protect

    private MinedItemRegistry reg;
    private float flatTimer;
    private RisingItem risingItem;

    public string tagName => "economy";

    private void Awake() {
        this.reg = Main.instance.itemRegistry;

        this.func();
    }

    private void Update() {
        if(Pause.isPaused()) {
            return;
        }

        if(this.risingItem == null) {
            this.flatTimer -= Time.deltaTime;

            if(this.flatTimer <= 0) {
                // Start a new spike.
                List<Item> list = this.getUnlockedItems();

                this.risingItem = new RisingItem(
                    list[Random.Range(0, list.Count)],
                    Random.Range(this._curveLengthRange.x, this._curveLengthRange.y));
            }
        } else {
            this.risingItem.timer -= Time.deltaTime;

            // This price change is over
            if(this.risingItem.timer <= 0) {
                this.risingItem = null;
                this.func();
            }
        }
    }

    /// <summary>
    /// Returns the value of the passed item adjusted based on the economy.
    /// </summary>
    public int getItemValue(Item item) {
        if(item.includeInEconemy) {

            float modifier = 1;

            if(this.risingItem != null && this.risingItem.item == item) {
                float curveValue = this._curve.Evaluate(1 - (this.risingItem.timer / this.risingItem.curveLength));
                curveValue = 1 + (curveValue * this._curvePriceMultiplyer);
                modifier = curveValue;
            }

            return Mathf.RoundToInt(item.moneyValue * modifier);
        } else {
            return item.moneyValue;
        }
    }

    public int getItemMaxValue(Item item) {
        return Mathf.RoundToInt(item.moneyValue * (1 + this._curvePriceMultiplyer));
    }

    /// <summary>
    /// Returns the value of the passed item ignoring the economy.
    /// </summary>
    public int getItemValueUnmodified(Item item) {
        return item.moneyValue;        
    }

    /// <summary>
    /// Sells the passed item for it's current value.
    /// </summary>
    public void sellItem(Item item) {
        if(item != null) {
            this._world.money.value += this.getItemValue(item);
        }
    }

    public bool isItemUnlocked(Item item) {
        MapGenerator generator = this._world.mapGenerator;
        for(int depth = 0; depth < generator.layerCount; depth++) {
            if(this._world.isDepthUnlocked(depth)) {
                OreSettings[] oreSettings = generator.getLayerFromDepth(depth).oreSpawnSettings;

                if(oreSettings != null) {
                    foreach(OreSettings setting in oreSettings) {
                        if(setting.cell != null && setting.cell is CellDataMineable) {
                            Item droppedItem = ((CellDataMineable)setting.cell).droppedItem;

                            if(item == droppedItem) {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Returns a list of all of the Items in the economy.
    /// </summary>
    public List<Item> getAllItems() {
        List<Item> list = new List<Item>();
        for(int id = 0; id < this.reg.getRegistrySize(); id++) {
            Item item = this.reg.getElement(id);

            if(item == null) {
                continue;
            }

            if(item.includeInEconemy) {
                list.Add(this.reg.getElement(id));
            }
        }

        return list;
    }

    /// <summary>
    /// Returns a list of all of the Items in the economy that are
    /// unlocked.  Unlocked meaning they occur on a layer that is
    /// unlocked.
    /// </summary>
    public List<Item> getUnlockedItems() {
        List<Item> items = new List<Item>();

        for(int id = 0; id < this.reg.getRegistrySize(); id++) {
            Item item = this.reg.getElement(id);

            if(item == null) {
                continue;
            }

            if(!item.includeInEconemy) {
                continue;
            }

            if(this.isItemUnlocked(item)) {
                items.Add(item);
            }
        }

        return items;
    }

    public void readFromNbt(NbtCompound tag) {
        if(tag.hasKey("risingItemId")) {
            this.risingItem = new RisingItem(
                this.reg.getElement(tag.getInt("risingItemId")),
                tag.getFloat("risingCurveLength"));
            this.risingItem.timer = tag.getFloat("risingTimer");
        }
    }

    public void writeToNbt(NbtCompound tag) {
        if(this.risingItem != null) {
            tag.setTag("risingItemId", this.reg.getIdOfElement(this.risingItem.item));
            tag.setTag("risingCurveLength", this.risingItem.curveLength);
            tag.setTag("risingTimer", this.risingItem.timer);
        }
    }

    private void func() {
        this.flatTimer = Random.Range(this._flatMarketTime.x, this._flatMarketTime.y);
    }

    [System.Serializable]
    private class RisingItem {

        public Item item;
        public float curveLength;
        public float timer;

        public RisingItem(Item item, float curveLength) {
            this.item = item;
            this.curveLength = curveLength;
            this.timer = curveLength;
        }
    }
}
