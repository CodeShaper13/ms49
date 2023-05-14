using UnityEngine;
using System.Collections.Generic;
using fNbt;

public class StatisticManager : MonoBehaviour, ISaveableState {

    private const string DOT_TIMES_BUILT = ".timesBuilt";
    private const string DOT_TIMES_MINED = ".timesMined";
    private const string DOT_TIMES_DESTROYED = ".timesDestroyed";

    public List<RegisteredStat> registeredStats {
        get; private set;
    }

    // fog lifted
    public StatisticInt leversFlipped;
    public StatisticInt minecartsPlaced;
    public StatisticInt minecartsDestroyed;

    public StatisticInt workersHired;
    public StatisticInt workersFired;

    public string saveableTagName => "statistics";

    private void Awake() {
        this.registeredStats = new List<RegisteredStat>();

        // General Stats:
        this.leversFlipped = this.registerStat(new StatisticInt("Levers Flipped", "leversFlipped"), EnumStatisticCategory.GENERAL);
        this.minecartsPlaced = this.registerStat(new StatisticInt("Minecarts Placed", "minecartsPlaced"), EnumStatisticCategory.GENERAL);
        this.minecartsDestroyed = this.registerStat(new StatisticInt("Minecarts Destroyed", "minecartsDestroyed"), EnumStatisticCategory.GENERAL);

        // Worker Stats:
        this.workersHired = this.registerStat(new StatisticInt("Workers Hired", "workersHired"), EnumStatisticCategory.WORKERS);
        this.workersFired = this.registerStat(new StatisticInt("Workers Fired", "workersFired"), EnumStatisticCategory.WORKERS);

        // Tile Stats:
        CellDataRegistry reg = Main.instance.CellRegistry;
        for(int i = 0; i < reg.RegistrySize; i++) {
            CellData cell = reg[i];
            if(cell != null) {
                this.registerStat(new StatisticInt(cell.DisplayName + " built", cell.name + DOT_TIMES_BUILT), EnumStatisticCategory.TILES);

                if(cell is CellDataMineable) {
                    this.registerStat(new StatisticInt(cell.DisplayName + " mined", cell.name + DOT_TIMES_MINED), EnumStatisticCategory.TILES);
                }

                if(cell.IsDestroyable) {
                    this.registerStat(new StatisticInt(cell.DisplayName + " destroyed", cell.name + DOT_TIMES_DESTROYED), EnumStatisticCategory.TILES);
                }
            }
        }
    }

    public void WriteToNbt(NbtCompound tag) {
        foreach(RegisteredStat regStat in this.registeredStats) {
            if(regStat != null) {
                NbtCompound compound = new NbtCompound();
                regStat.stat.writeToNbt(compound);

                tag.setTag(regStat.stat.saveName, compound);
            }
        }
    }

    public void ReadFromNbt(NbtCompound tag) {
        foreach(NbtCompound compound in tag) {
            string compoundName = compound.Name;

            // Find the matching stat
            IStatistic stat = null;
            foreach(RegisteredStat regStat in this.registeredStats) {
                if(regStat != null && regStat.stat.saveName == compoundName) {
                    stat = regStat.stat;
                }
            }

            if(stat != null) {
                stat.readFromNbt(compound);
            }
        }
    }

    public StatisticInt getCellBuiltStat(CellData cell) {
        if(cell != null) {
            return this.getStatFromIdentifer<StatisticInt>("stat." + cell.name + DOT_TIMES_BUILT);
        }

        return null;
    }

    public StatisticInt getCellMinedStat(CellData cell) {
        if(cell != null) {
            return this.getStatFromIdentifer<StatisticInt>("stat." + cell.name + DOT_TIMES_MINED);
        }

        return null;
    }

    public StatisticInt getCellDestroyedStat(CellData cell) {
        if(cell != null) {
            return this.getStatFromIdentifer<StatisticInt>("stat." + cell.name + DOT_TIMES_DESTROYED);
        }

        return null;
    }

    /// <summary>
    /// Returns the stat with the passed identifier.  Null is
    /// returned if there is no matching stat or the stat is of the
    /// wrong type.
    /// </summary>
    private T getStatFromIdentifer<T>(string id) {
        foreach(RegisteredStat regStat in this.registeredStats) {
            if(regStat.stat.saveName == id) {
                if(regStat.stat is T) {
                    return (T)regStat.stat;
                } else {
                    Debug.LogWarning("Stat with id \"" + id + "\" is of the wrong type");
                    break;
                }
            }
        }

        return default;
    }

    private T registerStat<T>(T stat, EnumStatisticCategory category = EnumStatisticCategory.GENERAL) where T : IStatistic {
        foreach(RegisteredStat s in this.registeredStats) {
            if(s.stat.saveName == stat.saveName) {
                Debug.LogWarning("Can not register stat with already used saveId of " + s.stat.saveName);
                return default;
            }
        }

        this.registeredStats.Add(new RegisteredStat(stat, category));

        return stat;
    }
}
