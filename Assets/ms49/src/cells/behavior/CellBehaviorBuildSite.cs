using fNbt;
using UnityEngine;
using System.Collections.Generic;

public class CellBehaviorBuildSite : CellBehaviorOccupiable, IHasData {

    [SerializeField]
    private ParticleSystem dustPs = null;

    public bool isPrimary { get; set; }
    public float constructionTime { get; private set; }

    private List<Entry> entires;

    public override void OnCreate(World world, CellState state, Position pos) {
        base.OnCreate(world, state, pos);

        this.entires = new List<Entry>();
    }

    public override string GetTooltipText() {
        foreach(Entry e in this.entires) {
            if(e.position == this.pos) {
                return e.cell.name; // TODO shows scriptable object name
            }
        }

        return null;
    }

    // Used for onDestroy()
    private static bool simpleRemove = false;

    public override void onDestroy() {
        base.onDestroy();

        if(!simpleRemove) {
            simpleRemove = true;

            if(this.isPrimary) {
                // Remove any "child" parts
                foreach(Entry e in this.entires) {
                    if(e.position != this.pos) {
                        this.world.SetCell(e.position, null);
                    }
                }
            } else {
                foreach(CellBehaviorBuildSite site in this.world.GetAllBehaviors<CellBehaviorBuildSite>()) {
                    if(site.isPrimary) {
                        // If the site is a primary and it contain this site (the one being destory and the child, remove it as well.
                        foreach(Entry otherSiteEntry in site.entires) {
                            if(otherSiteEntry.position == this.pos) {
                                simpleRemove = false;
                                this.world.SetCell(site.pos, null);
                                return;
                            }
                        }
                    }
                }
            }

            simpleRemove = false;
        }
    }

    public void ReadFromNbt(NbtCompound tag) {
        NbtList tagList = tag.getList("entries");
        foreach(NbtCompound compound in tagList) {
            this.entires.Add(new Entry(compound));
        }

        this.isPrimary = tag.getBool("isPrimary");
        this.constructionTime = tag.getFloat("constructionTime");
    }

    public void WriteToNbt(NbtCompound tag) {
        NbtList entriesTagList = new NbtList(NbtTagType.Compound);
        foreach(Entry e in this.entires) {
            entriesTagList.Add(e.writeToNbt());
        }
        tag.setTag("entries", entriesTagList);

        tag.setTag("isPrimary", this.isPrimary);
        tag.setTag("constructionTime", this.constructionTime);
    }

    /// <summary>
    /// Plays the building dust could particle effect.
    /// </summary>
    public void startPs() {
        this.dustPs.Play();
    }

    public void stopPs() {
        this.dustPs.Stop();
    }

    public void setPrimary(CellData cell, float constructionTime, bool addFog) {
        this.isPrimary = true;
        this.constructionTime = constructionTime;
        this.entires.Add(new Entry(cell, this.pos, addFog));
    }

    /// <summary>
    /// Adds a child build site to this site.
    /// </summary>
    public void addChildBuildSite(CellData cell, Position pos, bool addFog) {
        this.entires.Add(new Entry(cell, pos, addFog));
    }

    /// <summary>
    /// Places the BuildSite's cell and all of it's child sites into the world.
    /// </summary>
    public void placeIntoWorld() {
        foreach(Entry e in this.entires) {
            this.world.SetCell(e.position, e.cell, this.rotation);
            if(e.addFogOnComplete) {
                this.world.PlaceFog(e.position);
            }
        }
    }

    private class Entry {

        public readonly CellData cell;
        public readonly Position position;
        public readonly bool addFogOnComplete;

        public Entry(NbtCompound tag) {
            this.cell = Main.instance.CellRegistry[tag.getInt("cellId")];
            this.position = new Position(tag.getVector3Int("offset"));
            this.addFogOnComplete = tag.getBool("addFog");
        }

        public Entry(CellData cell, Position offset, bool addFog) {
            this.cell = cell;
            this.position = offset;
            this.addFogOnComplete = addFog;
        }

        public NbtCompound writeToNbt() {
            NbtCompound tag = new NbtCompound();
            tag.setTag("cellId", Main.instance.CellRegistry.GetIdOfElement(this.cell));
            tag.setTag("offset", this.position.AsVec3Int);
            tag.setTag("addFog", this.addFogOnComplete);
            return tag;
        }
    }
}
