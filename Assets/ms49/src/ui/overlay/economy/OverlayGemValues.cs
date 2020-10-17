using System.Collections.Generic;
using UnityEngine;

public class OverlayGemValues : MonoBehaviour {

    [SerializeField]
    private RectTransform _area = null;
    [SerializeField]
    private GameObject _barPrefab = null;
    [SerializeField]
    private bool showLockedItems = true;

    private World world;
    private Economy economy;

    private OverlayGemValueBar[] bars;

    private void Start() {
        this.world = GameObject.FindObjectOfType<World>();
        this.economy = world.economy;

        List<Item> items = this.economy.getAllItems();
        this.bars = new OverlayGemValueBar[items.Count];

        for(int i = 0; i < items.Count; i++) {
            OverlayGemValueBar bar =
                GameObject.Instantiate(
                    this._barPrefab,
                    this._area).GetComponent<OverlayGemValueBar>();

            bar.setItem(items[i]);

            this.bars[i] = bar;
        }
    }

    private void Update() {
        foreach(OverlayGemValueBar bar in this.bars) {
            bool flag = this.showLockedItems || this.economy.isItemUnlocked(bar.item);
            bar.gameObject.SetActive(flag);
            if(flag) {
                bar.update(this.economy);
            }
        }
    }
}
