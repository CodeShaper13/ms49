using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupStatistics : PopupWorldReference {

    [SerializeField]
    private RectTransform _layoutRect = null;
    [SerializeField]
    private Scrollbar _scrollbar = null;
    [SerializeField]
    private Toggle _toggle = null;
    [SerializeField]
    private GameObject _statDisplayElementPrefab = null;

    private List<UiStatisticEntry> statLables;
    private EnumStatisticCategory currentCategory;
    private bool hideEmptyStats;

    protected override void initialize() {
        base.initialize();

        this.statLables = new List<UiStatisticEntry>();

        int i = 0;
        foreach(RegisteredStat stat in this.world.statManager.registeredStats) {
            if(stat != null) {
                UiStatisticEntry sut = GameObject.Instantiate(this._statDisplayElementPrefab, this._layoutRect).GetComponent<UiStatisticEntry>();
                sut.setStat(stat);

                this.statLables.Add(sut);

                i++;
            }
        }
    }

    protected override void onOpen() {
        base.onOpen();

        this.callback_setViewedStatTab(0);
    }

    public void callback_toggleTick() {
        this.hideEmptyStats = !this._toggle.isOn;

        this.func();
    }

    public void callback_setViewedStatTab(int category) {
        this.currentCategory = (EnumStatisticCategory)category;

        this.func();

        this._scrollbar.value = 1f;
    }

    private void func() {
        foreach(UiStatisticEntry entry in this.statLables) {
            entry.gameObject.SetActive(entry.category == this.currentCategory && (!this.hideEmptyStats || !this.isZero(entry.stat)));
        }

        // Alternate the label colors.
        int i = 0;
        foreach(UiStatisticEntry entry in this.statLables) {
            if(entry.gameObject.activeSelf) {
                entry.setColor(i % 2 == 0);
                i++;
            }
        }
    }

    private bool isZero(IStatistic stat) {
        if(stat is StatisticInt) {
            return ((StatisticInt)stat).get() == 0;
        } else if(stat is StatisticFloat) {
            return ((StatisticFloat)stat).get() == 0;
        } else {
            return false;
        }
    }
}
