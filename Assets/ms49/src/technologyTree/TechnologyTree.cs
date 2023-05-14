using fNbt;

using System.Collections.Generic;
using UnityEngine;
using XNode;
using static NodeTechTree;

public class TechnologyTree : MonoBehaviour, ISaveableState {

    private const string DEFAULT_NODE_NAME = "DEFAULT";

    [SerializeField]
    private NodeGraphTechTree _graph = null;

    private Dictionary<NodeTechTree, bool> technologyLockStates;

    /// <summary>
    /// The technology being researched.
    /// </summary>
    public ResearchProgress targetTechnology { get; private set; }

    public List<Node> Nodes => this._graph.nodes;
    public bool IsResearching => this.targetTechnology != null;
    public string saveableTagName => "technologyTree";

    private void Awake() {
        this.technologyLockStates = new Dictionary<NodeTechTree, bool>();

        // Populate the dictionary.  
        foreach(var node in this._graph.nodes) {
            this.technologyLockStates.Add((NodeTechTree)node, false);
        }

        // Unlock the starting unlockables.
        this.SetTechnologyUnlocked(this.GetNode(DEFAULT_NODE_NAME), false);
    }

    public void ReadFromNbt(NbtCompound tag) {
        NbtCompound tagUnlockStates = tag.getCompound("unlockState");

        foreach(var node in this._graph.nodes) {
            if(node is NodeTechTree techNode) {
                techNode.IsUnlocked = tagUnlockStates.getBool(techNode.SaveName);
            }
        }
    }

    public void WriteToNbt(NbtCompound tag) {
        NbtCompound tagUnlockStates = new NbtCompound();

        foreach(var node in this._graph.nodes) {
            if(node is NodeTechTree techNode) {
                tagUnlockStates.setTag(techNode.SaveName, techNode.IsUnlocked);
            }
        }

        tag.setTag("unlockState", tagUnlockStates);
    }

    public void SetTargetResearch(NodeTechTree technology) {
        if(this.IsResearching) {
            Debug.LogWarning("There is already a research in progress");
            return;
        }

        if(this.IsTechnologyUnlocked(technology)) {
            Debug.LogWarning("Can not research technology, it is already unlocked.");
            return;
        }

        this.targetTechnology = new ResearchProgress(technology);
    }

    public bool AddResearch(Item item) {
        if(!this.IsResearching) {
            Debug.LogWarning("Can't add research, there are no researches in progress.");
            return false;
        }

        // Make sure the passed item is one of the needed items.
        if(this.targetTechnology.NeedsItem(item)) {
            this.targetTechnology.AddItem(item);
            return true;
        }

        if(this.targetTechnology.AllCategoriesFilled()) {
            // Research done!
            this.SetTechnologyUnlocked(this.targetTechnology.technology);
            this.targetTechnology = null;
        }

        return false;
    }

    public void CancelResearch() {
        this.targetTechnology = null;
    }

    public bool IsTechnologyUnlocked(NodeTechTree node) {
        if(this.technologyLockStates.TryGetValue(node, out bool value)) {
            return value;
        }

        Debug.Log("Passed node is not in the Tech Tree.");

        return false;
    }

    /// <summary>
    /// Unlocks the passed Technology.  If if is already unlocked,
    /// nothing happens.
    /// </summary>
    public void SetTechnologyUnlocked(NodeTechTree node, bool updateUi = true) {
        if(this.technologyLockStates.TryGetValue(node, out bool value) && !value) {
            this.technologyLockStates[node] = true;

            // Apply the fields
            foreach(NodeTechTree.Field field in node.UnlockedFields) {
                if(field == null) {
                    continue;
                }
                
                field.Apply();
            }

            if(updateUi) {
                PopupTechnologyTree tree = Main.instance.findPopup<PopupTechnologyTree>();

            }
        } else {
            Debug.LogFormat("Passed node, \"{0}\", is not in the Tech Tree, it can not be unlocked.", node.name);
        }
    }

    private NodeTechTree GetNode(string saveName) {
        foreach(var node in this._graph.nodes) {
            if(node is NodeTechTree techNode) {
                if(techNode.SaveName == saveName) {
                    return techNode;
                }
            }
        }

        return null;
    }

    public class ResearchProgress {

        public readonly NodeTechTree technology;
        public readonly int[] itemsContributed;

        public ResearchProgress(NodeTechTree technology) {
            this.technology = technology;
            this.itemsContributed = new int[technology.Costs.Length];
        }

        public bool AllCategoriesFilled() {
            for(int i = 0; i < this.technology.Costs.Length; i++) {
                if(this.itemsContributed[i] < this.technology.Costs[i].cost) {
                    return false;
                }
            }

            return true;
        }

        public bool NeedsItem(Item item) {
            int index = this.GetIndex(item);

            if(index == -1) {
                return false;
            }

            UnlockCost cost = this.technology.Costs[index];

            return this.itemsContributed[index] < cost.cost;
        }

        public void AddItem(Item item) {
            int index = this.GetIndex(item);

            if(index == -1) {
                return;
            }

            this.itemsContributed[index] += 1;
        }

        private int GetIndex(Item item) {
            for(int i = 0; i < this.technology.Costs.Length; i++) {
                if(this.technology.Costs[i].item == item) {
                    return i;
                }
            }

            return 0;
        }
    }
}