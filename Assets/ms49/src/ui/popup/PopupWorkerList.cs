using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopupWorkerList : PopupWorldReference {

    [SerializeField]
    private Text textWorkerCount = null;
    [SerializeField]
    private Text textWorkerCost = null;
    [SerializeField]
    private RectTransform btnArea = null;
    [SerializeField]
    private GameObject btnPrefab = null;

    private List<WorkerListButton> workerButtons;

    protected override void initialize() {
        base.initialize();

        this.workerButtons = new List<WorkerListButton>();
    }

    protected override void onUpdate() {
        base.onUpdate();

        this.updateBtnList();
        this.updateTopText();
    }

    private void updateBtnList() {
        // Remove buttons for Workers that no longer exist.
        for(int i = this.workerButtons.Count - 1; i >= 0; i--) {
            WorkerListButton btn = this.workerButtons[i];
            if(btn.worker == null) { // || btn.worker.isDead) {
                // Worker is either gone or dead
                this.workerButtons.RemoveAt(i);
                GameObject.Destroy(btn.gameObject);

            }
        }

        // Create buttons for Worker's not in the list.
        foreach(EntityBase e in this.world.entities.list) {
            if(e is EntityWorker) {
                EntityWorker worker = (EntityWorker)e;

                if(!this.workerButtons.Any(btn => btn.worker == worker)) {
                    WorkerListButton newButton = GameObject.Instantiate(this.btnPrefab, this.btnArea).GetComponent<WorkerListButton>();
                    newButton.setWorker(worker);
                    this.workerButtons.Add(newButton);
                }
            }
        }
    }

    private void updateTopText() {
        // Update Text
        int workerCount = 0;
        int totalCost = 0;

        foreach(EntityBase e in this.world.entities.list) {
            if(e is EntityWorker) {
                EntityWorker worker = (EntityWorker)e;
                if(!worker.isDead) {
                    workerCount++;
                    totalCost += worker.info.pay;
                }
            }
        }

        this.textWorkerCount.text = "Workers: " + workerCount;
        this.textWorkerCost.text = "Cost Per Day: $" + totalCost;
    }
}
