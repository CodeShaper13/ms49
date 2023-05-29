using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PopupWorkerList : PopupWorldReference {

    [SerializeField, Required]
    private TMP_Text _textWorkerCount = null;
    [SerializeField, Required]
    private TMP_Text _textWorkerCost = null;
    [SerializeField, Required]
    private RectTransform _workerEntryArea = null;
    [SerializeField, Required]
    private GameObject _workEntryPrefab = null;

    private List<WorkerListButton> workerButtons;

    private void Awake() {
        this.workerButtons = new List<WorkerListButton>();
    }

    protected override void onUpdate() {
        base.onUpdate();

        this.UpdateBtnList();
        this.UpdateTopText();
    }

    private void UpdateBtnList() {
        // Remove buttons for Workers that no longer exist.
        for(int i = this.workerButtons.Count - 1; i >= 0; i--) {
            WorkerListButton btn = this.workerButtons[i];
            if(btn.worker == null) { // || btn.worker.isDead) {
                // Worker is either gone or dead
                this.workerButtons.RemoveAt(i);
                GameObject.Destroy(btn.gameObject);

            }
        }

        // Create buttons for Workers not in the list.
        foreach(EntityBase entity in this.world.entities.list) {
            if(entity is EntityWorker entityWorker) {
                if(!this.workerButtons.Any(btn => btn.worker == entityWorker)) {
                    WorkerListButton newButton = GameObject.Instantiate(this._workEntryPrefab, this._workerEntryArea).GetComponent<WorkerListButton>();
                    newButton.gameObject.SetActive(true);
                    newButton.setWorker(entityWorker);
                    this.workerButtons.Add(newButton);
                }
            }
        }
    }

    private void UpdateTopText() {
        // Update Text
        int workerCount = 0;
        int totalCost = 0;

        foreach(EntityBase e in this.world.entities.list) {
            if(e is EntityWorker entityWorker) {
                if(!entityWorker.isDead) {
                    workerCount++;
                    totalCost += entityWorker.info.pay;
                }
            }
        }

        this._textWorkerCount.text = string.Format("Worker Count: {0}", workerCount);
        this._textWorkerCost.text = string.Format("Cost per pay period: ${0}", totalCost);
    }
}
