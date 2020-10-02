using System.Collections.Generic;
using UnityEngine;

public class PopupOreValues : PopupWorldReference {

    [SerializeField]
    private GameObject _valueBarPrefab = null;
    [SerializeField]
    private RectTransform _barParent = null;

    private List<ValueBar> bars;

    protected override void initialize() {
        base.initialize();

        this.bars = new List<ValueBar>();
    }

    protected override void onOpen() {
        base.onOpen();

        this.createBars();
    }

    protected override void onClose() {
        base.onClose();

        // Destroy all of the Bars
        foreach(Transform t in this._barParent) {
            GameObject.Destroy(t.gameObject);
        }

        this.bars.Clear();
    }

    private void createBars() {
        this.bars = new List<ValueBar>();

        for(int id = 0; id < Main.instance.itemRegistry.getRegistrySize(); id++) {
            Item item = Main.instance.itemRegistry.getElement(id);

            if(item == null) {
                continue;
            }

            if(!item.includeInEconemy) {
                continue;
            }

            // Check if the ore is generated in an unlocked layer

            MapGenerationData genData = this.world.mapGenData;
            for(int depth = 0; depth < genData.layerCount; depth++) {
                if(this.world.isDepthUnlocked(depth)) {
                    OreSettings[] oreSettings = genData.getLayerFromDepth(depth).oreSpawnSettings;

                    if(oreSettings != null) {
                        foreach(OreSettings setting in oreSettings) {
                            if(setting.cell != null && setting.cell is CellDataMineable) {
                                Item droppedItem = ((CellDataMineable)setting.cell).droppedItem;

                                if(item == droppedItem) {
                                    // Add a bar, this ore has been unlocked
                                    ValueBar bar = GameObject.Instantiate(this._valueBarPrefab, this._barParent).GetComponent<ValueBar>();
                                    bar.setItem(item);
                                    this.bars.Add(bar);

                                    goto label1;
                                }
                            }
                        }
                    }
                }
            }

            label1: ;
        }
    }
}
