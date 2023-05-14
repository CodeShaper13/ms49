using TMPro;
using UnityEngine;

public class PopupTechnologyTree : PopupWindow {

    [SerializeField]
    private NodeGraphTechTree _researchTree = null;
    [SerializeField]
    private TechTreeDrawer _techTreeDrawer = null;
    [SerializeField]
    private TMP_Text _textTargetTechnology = null;

    private void Start() {
        this._techTreeDrawer.Clear();
        this._techTreeDrawer.Generate();
    }

    protected override void onOpen() {
        base.onOpen();

        if(this._researchTree == null) {
            Debug.LogError("ResearchTree must be assigned in inspector");
            return;
        }
    }

    public void Callback_CancelResearch() {
        Main.instance.activeWorld.technologyTree.CancelResearch();
        this._textTargetTechnology.text = null;
    }
}