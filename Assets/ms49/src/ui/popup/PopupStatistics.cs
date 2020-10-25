using UnityEngine;

public class PopupStatistics : PopupWorldReference {

    [SerializeField]
    private RectTransform _layoutRect = null;
    [SerializeField]
    private GameObject _statDisplayElementPrefab = null;

    protected override void initialize() {
        base.initialize();

        int i = 0;
        foreach(IStatistic stat in this.world.statManager.statistics) {
            if(stat != null) {
                UiStatisticEntry sut = GameObject.Instantiate(this._statDisplayElementPrefab, this._layoutRect).GetComponent<UiStatisticEntry>();
                sut.setStat(stat, i % 2 == 0);

                i++;
            }
        }
    }
}
