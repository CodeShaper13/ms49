using fNbt;
using UnityEngine;
using System.Collections.Generic;

public class CellBehaviorBuildSite : CellBehaviorOccupiable, IHasData {

    [SerializeField]
    private ParticleSystem dustPs = null;

    public bool isPrimary { get; set; }

    private List<Entry> entires;

    public override void onCreate(World world, CellState state, Position pos) {
        base.onCreate(world, state, pos);

        this.entires = new List<Entry>();
    }

    public void readFromNbt(NbtCompound tag) {
        NbtList tagList = tag.getList("entries");
        foreach(NbtCompound compound in tagList) {
            this.entires.Add(new Entry(compound));
        }

        this.isPrimary = tag.getBool("isPrimary");
    }

    public void writeToNbt(NbtCompound tag) {
        NbtList entriesTagList = new NbtList(NbtTagType.Compound);
        foreach(Entry e in this.entires) {
            entriesTagList.Add(e.writeToNbt());
        }
        tag.setTag("entries", entriesTagList);

        tag.setTag("isPrimary", this.isPrimary);
    }

    public void startPs() {
        this.dustPs.Play();
    }

    public void addCell(CellData cell, Position pos) {
        this.entires.Add(new Entry(cell, pos));
    }

    /// <summary>
    /// Places the save Cell into the world.
    /// </summary>
    public void placeIntoWorld() {
        foreach(Entry e in this.entires) {
            this.world.setCell(e.position, e.cell, this.rotation);
        }
    }

    private class Entry {

        public readonly CellData cell;
        public readonly Position position;

        public Entry(NbtCompound tag) {
            this.cell = Main.instance.tileRegistry.getElement(tag.getInt("cellId"));
            this.position = new Position(tag.getVector3Int("offset"));
        }

        public Entry(CellData cell, Position offset) {
            this.cell = cell;
            this.position = offset;
        }

        public NbtCompound writeToNbt() {
            NbtCompound tag = new NbtCompound();
            tag.setTag("cellId", Main.instance.tileRegistry.getIdOfElement(this.cell));
            tag.setTag("offset", this.position.vec3Int);
            return tag;
        }
    }
}
