using UnityEngine;
using System.Collections.Generic;
using fNbt;

public class StatisticManager : MonoBehaviour, ISaveableState {

    public List<IStatistic> statistics {
        get; private set;
    }

    public StatisticInt workersHired;
    public StatisticInt workersFired;

    public string tagName => "statistics";

    private void Awake() {
        this.statistics = new List<IStatistic>();

        this.workersHired = this.registerStat(new StatisticInt("Workers Hired", "workersHired"));
        this.workersFired = this.registerStat(new StatisticInt("Workers Fired", "workersFired"));
    }

    public void writeToNbt(NbtCompound tag) {
        foreach(IStatistic stat in this.statistics) {
            if(stat != null) {
                NbtCompound compound = new NbtCompound();
                stat.writeToNbt(compound);

                tag.setTag(stat.saveName, compound);
            }
        }
    }

    public void readFromNbt(NbtCompound tag) {
        foreach(NbtCompound compound in tag) {
            string compoundName = compound.Name;

            // Find the matching stat
            IStatistic stat = null;
            foreach(IStatistic s in this.statistics) {
                if(s != null && s.saveName == compoundName) {
                    stat = s;
                }
            }

            if(stat != null) {
                stat.readFromNbt(compound);
            }
        }
    }

    private T registerStat<T>(T stat) where T : IStatistic {
        this.statistics.Add(stat);

        return stat;
    }
}
