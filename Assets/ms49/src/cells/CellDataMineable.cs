using UnityEngine;

[CreateAssetMenu(fileName = "Cell", menuName = "MS49/Cell/Cell Mineable", order = 1)]
public class CellDataMineable : CellData {

    [SerializeField]
    private Item droppedItem = null;
    [SerializeField]
    private bool _showParticles = true;

    public bool ShowParticles => this._showParticles;
    public Item DroppedItem => this.droppedItem;
}
