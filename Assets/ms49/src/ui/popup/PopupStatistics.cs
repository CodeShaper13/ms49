using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupStatistics : PopupWindow {

    [SerializeField]
    private RectTransform _layoutRect = null;
    [SerializeField]
    private Scrollbar _scrollbar = null;
    [SerializeField]
    private Toggle _toggle = null;
    [SerializeField]
    private GameObject _statDisplayElementPrefab = null;

    [Space]
    public Color brightColor = Color.white;
    public Color darkColor = Color.gray;

    private List<StatListEntry> statEntries;
    private EnumStatisticCategory currentCategory;
    private bool hideEmptyStats;

    private void Awake() {
        this.statEntries = new List<StatListEntry>();

        foreach(RegisteredStat stat in Main.instance.activeWorld.statManager.registeredStats) {
            if(stat != null) {
                GameObject obj = GameObject.Instantiate(
                    this._statDisplayElementPrefab,
                    this._layoutRect);
                this.statEntries.Add(new StatListEntry(obj, stat));
            }
        }
    }

    protected override void onOpen() {
        base.onOpen();

        foreach(StatListEntry entry in this.statEntries) {
            entry.Refresh();
        }

        this.SetVisibleTab(EnumStatisticCategory.GENERAL);
    }

    public void Callback_ToggleHideEmptyStats() {
        this.hideEmptyStats = !this._toggle.isOn;

        this.RedrawList();
    }

    public void Callback_SetViewedStatTab(int category) {
        this.SetVisibleTab((EnumStatisticCategory)category);
    }

    public void SetVisibleTab(EnumStatisticCategory category) {
        this.currentCategory = category;
        this.RedrawList();
        this._scrollbar.value = 1f;
    }

    private void RedrawList() {
        foreach(StatListEntry entry in this.statEntries) {
            entry.gameObject.SetActive(entry.category == this.currentCategory && (!this.hideEmptyStats || !this.IsZero(entry.stat)));
        }

        // Alternate the label colors.
        int i = 0;
        foreach(StatListEntry entry in this.statEntries) {
            if(entry.gameObject.activeSelf) {
                entry.SetColor(i % 2 == 0 ? this.brightColor : this.darkColor);
                i++;
            }
        }
    }

    /// <summary>
    /// Checks if the passed stat is 0 and should be hidden (if option is checked).
    /// </summary>
    private bool IsZero(IStatistic stat) {
        if(stat is StatisticInt statInt) {
            return statInt.get() == 0;
        } else if(stat is StatisticFloat statFloat) {
            return statFloat.get() == 0;
        } else {
            return false;
        }
    }

    private class StatListEntry {

        private readonly TMP_Text textName;
        private readonly TMP_Text textValue;

        public readonly GameObject gameObject;
        public readonly IStatistic stat;
        public readonly EnumStatisticCategory category;

        public StatListEntry(GameObject gameObj, RegisteredStat registeredStat) {
            this.gameObject = gameObj;
            this.stat = registeredStat.stat;
            this.category = registeredStat.category;
            
            this.textName = gameObj.transform.GetChild(0).GetComponent<TMP_Text>();
            this.textValue = gameObj.transform.GetChild(1).GetComponent<TMP_Text>();

            this.textName.text = this.stat.displayName;

            this.Refresh();
        }

        public void Refresh() {
            this.textValue.text = this.stat.displayValue;
        }

        public void SetColor(Color color) {
            this.textName.color = color;
            this.textValue.color = color;
        }
    }
}
