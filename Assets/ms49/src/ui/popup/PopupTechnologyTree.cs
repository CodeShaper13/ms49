using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupTechnologyTree : PopupWorldReference {

    [SerializeField]
    private TechTreeDrawer _techTreeDrawer = null;
    [SerializeField]
    private TMP_Text _textTargetTechnology = null;
    [SerializeField, Required]
    private Image _imgTechnologyIcon = null;
    [SerializeField, Required]
    private GameObject _priceLabel = null;
    [SerializeField, Required]
    private Button _buttonStartResearch = null;
    [SerializeField, Required]
    private Button _buttonStopResearch = null;

    private Dictionary<Item, ProgressBar> progressBars;
    private NodeTechTree selectedTechnology;

    protected override void Start() {
        base.Start();

        this._techTreeDrawer.Clear();
        this._techTreeDrawer.Generate();

        this.progressBars = new Dictionary<Item, ProgressBar>();
        foreach(Item item in Main.instance.ItemRegistry) {
            ProgressBar progressBar = GameObject.Instantiate(this._priceLabel, this._priceLabel.transform.parent).GetComponent<ProgressBar>();
            Image img = progressBar.transform.GetChild(2)?.GetComponent<Image>();
            if(img != null) {
                img.sprite = item.sprite;
            }

            this.progressBars.Add(item, progressBar);
        }
    }

    protected override void onOpen() {
        base.onOpen();
        
        if(this.world.technologyTree.IsResearching) {
            this.SetSelectedTechnology(this.world.technologyTree.targetTechnology.technology);
        }
    }

    protected override void onUpdate() {
        base.onUpdate();

        TechnologyTree techTree = this.world.technologyTree;
        bool techIsSelected = this.selectedTechnology != null;
        bool selectedIsBeingResearched = techTree.IsResearching && techTree.targetTechnology.technology == this.selectedTechnology;
        bool isSelectedAvailable = techIsSelected && techTree.IsTechnologyAvailable(this.selectedTechnology);

        if(techIsSelected) {
            bool alreadyUnlocked = techTree.IsTechnologyUnlocked(this.selectedTechnology);

            foreach(var unlockCost in this.selectedTechnology.Costs) {
                if(this.progressBars.TryGetValue(unlockCost.item, out ProgressBar bar)) {
                    if(alreadyUnlocked) {
                        // Fill the bar.
                        bar.Value = bar.maxValue;
                    }
                    else {
                        // Show progress.
                        bar.Value = selectedIsBeingResearched ? techTree.targetTechnology.GetItemsContributed(unlockCost.item) : 0;
                    }
                }
            }
        }

        this._buttonStartResearch.interactable = techIsSelected && !techTree.IsResearching && isSelectedAvailable;
        this._buttonStopResearch.gameObject.SetActive(techIsSelected && selectedIsBeingResearched);

        if(techIsSelected) {
            string s;
            if(selectedIsBeingResearched) {
                s = "{0}\n(Researching)";
            } else if(isSelectedAvailable) {
                s = "{0}\n(Available)";
            } else {
                s = "{0}\n(Locked)";
            }
            this._textTargetTechnology.text = string.Format(s, this.selectedTechnology.TechName);
        } else {
            this._textTargetTechnology.text = "Selected a Technology";
        }

        this._imgTechnologyIcon.sprite = techIsSelected ? this.selectedTechnology.Image : null;
    }

    public void Callback_StartResearch() {
        this.world.technologyTree.SetTargetResearch(this.selectedTechnology);
    }

    public void Callback_CancelResearch() {
        this.world.technologyTree.CancelResearch();
    }

    public void Callback_SetSelectedTechnology(TechTreeTechnology technology) {
        if(!this.world.technologyTree.IsResearching) {
            this.SetSelectedTechnology(technology.Node);
        }
    }

    private void SetSelectedTechnology(NodeTechTree node) {
        this.selectedTechnology = node;

        // Disable all progress bars.
        foreach(var pair in this.progressBars) {
            pair.Value.gameObject.SetActive(false);
        }

        foreach(var unlockCost in node.Costs) {
            if(this.progressBars.TryGetValue(unlockCost.item, out ProgressBar bar)) {
                bar.gameObject.SetActive(true);
                bar.maxValue = unlockCost.cost;
            }
        }
    }
}