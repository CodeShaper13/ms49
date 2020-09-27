using UnityEngine;

public class WorkerFactory : MonoBehaviour {

    [SerializeField]
    private int _workerEntityId = 1;
    [SerializeField]
    private Color[] _skinTones = null;
    [SerializeField]
    private DirectionalSprites[] _hairSprites = null;
    [SerializeField, MinMaxSlider(0, 100)]
    private Vector2Int _payRange = new Vector2Int(35, 60);

    public Color[] skinTones => this._skinTones;
    public DirectionalSprites[] hairSprites => this._hairSprites;
    public Vector2Int payRange => this._payRange;

    public WorkerInfo generateWorkerInfo() {
        string first;
        string last;
        Main.instance.names.getRandomName(EnumGender.MALE, out first, out last);

        return new WorkerInfo(
            first,
            last,
            EnumGender.MALE, // = Random.Range(0, 2) == 0 ? EnumGender.MALE : EnumGender.FEMALE;
            Random.Range(0, this._skinTones.Length),
            Random.Range(0, this._hairSprites.Length),
            0, // TODO hair style
            Random.Range(0, Main.instance.personalities.getPersonalityCount() - 1),
            Random.Range(0.8f, 1.2f),
            Random.Range(this._payRange.x, this._payRange.y + 1)
        );
    }

    public EntityWorker spawnWorker(World world, Position pos, WorkerInfo info, WorkerType type) {
        EntityBase e = world.entities.spawn(
            pos,
            this._workerEntityId);

        if(e is EntityWorker) {
            EntityWorker worker = (EntityWorker)e;
            worker.info = info;
            worker.setType(type);

            return worker;
        } else {
            return null;
        }
    }

    public DirectionalSprites getHairSpritesFromId(int hairColor) {
        return this._hairSprites[Mathf.Clamp(hairColor, 0, this._hairSprites.Length - 1)];
    }

    public Color getSkinColorFromTone(int tone) {
        return this._skinTones[Mathf.Clamp(tone, 0, this._skinTones.Length - 1)];
    }
}
