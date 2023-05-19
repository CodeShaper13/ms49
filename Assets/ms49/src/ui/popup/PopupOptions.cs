using UnityEngine;
using TMPro;

public class PopupOptions : PopupWindow {

    [SerializeField]
    private Options _options = null;
    [SerializeField]
    private RectTransform _optionControlParent = null;

    [Space]

    [SerializeField]
    private GameObject _sliderPrefab = null;
    [SerializeField]
    private GameObject _togglePrefab = null;

    private void Start() {
        // Create Ui Control elements for every option.
        foreach(IOption option in this._options.allOptions) {
            GameObject prefab = this.GetPrefab(option);

            if(prefab != null) {
                GameObject obj = GameObject.Instantiate(prefab, this._optionControlParent);
                obj.gameObject.SetActive(true);

                obj.GetComponentInChildren<TMP_Text>().text = option.name;

                option.setupControlObj(obj);
            }
        }
    }

    protected override void onClose() {
        base.onClose();

        this._options.saveOptions();
    }

    private GameObject GetPrefab(IOption option) {
        if(option is OptionSlider) {
            return this._sliderPrefab;
        }
        else if(option is OptionToggle) {
            return this._togglePrefab;
        }

        return null;
    }
}
