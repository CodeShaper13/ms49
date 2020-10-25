using UnityEngine;

[CreateAssetMenu(fileName = "Cell", menuName = "MS49/Cell/Cell Mineable", order = 1)]
public class CellDataMineable : CellData {

    public Item droppedItem;
    [SerializeField]
    private bool _showParticles = true;

    public bool showParticles => this._showParticles;
}
