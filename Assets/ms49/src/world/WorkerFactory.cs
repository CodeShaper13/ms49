using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorkerFactory : MonoBehaviour {

    [SerializeField]
    private Names _names = null;
    [SerializeField]
    private GameObject _workerPrefab = null;
    [SerializeField]
    private Color[] _skinTones = null;
    [SerializeField]
    private DirectionalSprites[] _hairSprites = null;
    [SerializeField, MinMaxSlider(-30, 30), Tooltip("A Workers pay is shifted randomly within this range (inclusive) to as variaty")]
    private Vector2Int _payWiggleRange = new Vector2Int(-3, 3);
    [SerializeField]
    private WorkerPayAmount _workerPay = new WorkerPayAmount(30, 40, 50);

    public Color[] skinTones => this._skinTones;
    public DirectionalSprites[] hairSprites => this._hairSprites;
    public Vector2Int payRange => this._payWiggleRange;

    public WorkerInfo generateWorkerInfo(WorkerType type, Personality forcedPersonality = null) {
        string first;
        string last;
        this._names.getRandomName(EnumGender.MALE, out first, out last);

        return new WorkerInfo(
            first,
            last,
            EnumGender.MALE, // = Random.Range(0, 2) == 0 ? EnumGender.MALE : EnumGender.FEMALE;
            Random.Range(0, this._skinTones.Length),
            Random.Range(0, this._hairSprites.Length),
            0, // TODO hair style
            forcedPersonality == null ? this.getRndPersonality(type) : forcedPersonality,
            Random.Range(this._payWiggleRange.x, this._payWiggleRange.y + 1)
        );
    }

    public EntityWorker spawnWorker(World world, Position pos, WorkerInfo info, WorkerType type) {
        EntityBase e = world.entities.Spawn(
            pos,
            Main.instance.EntityRegistry.GetIdOfElement(this._workerPrefab));

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

    /// <summary>
    /// Returns a Worker's base pay off their pay modifier.  There
    /// payshift needs to be added to get their actual pay.
    /// </summary>
    public int getPayFromModifier(EnumPayModifier payMod) {
        switch(payMod) {
            case EnumPayModifier.LOW:
                return this._workerPay.low;
            case EnumPayModifier.HIGH:
                return this._workerPay.high;
            default:
                return this._workerPay.normal;
        }
    }

    private Personality getRndPersonality(WorkerType type) {
        List<Personality> list = new List<Personality>();

        for(int i = 0; i < Main.instance.PersonalityRegistry.RegistrySize; i++) {
            Personality p = Main.instance.PersonalityRegistry[i];

            if(p != null && p.canHave(type)) {
                list.Add(p);
            }
        }

        return list[Random.Range(0, list.Count - 1)];
    }

    [Serializable]
    private class WorkerPayAmount {
        public int low;
        public int normal;
        public int high;

        public WorkerPayAmount(int v1, int v2, int v3) {
            this.low = v1;
            this.normal = v2;
            this.high = v3;
        }
    }
}
