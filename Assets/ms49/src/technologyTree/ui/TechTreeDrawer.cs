using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

/// <summary>
/// Taken from https://rachel53461.wordpress.com/2014/04/20/algorithm-for-drawing-trees/
/// 
/// TODO adopt this instead: https://github.com/Unity-Technologies/graph-visualizer/blob/master/Editor/Graph/Layouts/ReingoldTilford.cs
/// </summary>
public class TechTreeDrawer : MonoBehaviour {

    public Node rootNode;

    [Space]

    public GameObject nodePrefab;
    public RectTransform nodeInstanceParent;

    [Space]

    public Vector2 nodeSize = new Vector2(144, 220);
    [Tooltip("How close siblings are to each other.")]
    public float siblingDistance = 0.0F;
    public float treeDistance = 0.0F;

    private List<TechTreeTechnology> nodeGameObjects = new List<TechTreeTechnology>();

    private void InitializeNodes(Technology node, int depth, List<Technology> allNodes) {
        allNodes.Add(node);

        foreach(NodePort child in node.graphNode.GetPort("_children").GetConnections()) {
            Technology childNode = new Technology(child.node, node, new Vector2Int(-1, depth));

            node.children.Add(childNode);

            this.InitializeNodes(childNode, depth + 1, allNodes);
        }
    }

    public void Generate(Node rootNode) {
        this.Clear();

        List<Technology> allNodes = new List<Technology>();

        Technology node = new Technology(rootNode, null, new Vector2Int(-1, 0));
        this.InitializeNodes(node, 1, allNodes);

        this.CalculateInitialX(node);
        //this.CheckAllChildrenOnScreen(node);
        this.CalculateFinalPositions(node, 0);

        this.CreateGameObjects(allNodes);

        Vector3 v = node.gameObject.transform.localPosition;
        foreach(Technology data in allNodes) {
            data.gameObject.transform.localPosition -= v;
        }
    }

    [Button]
    public void Generate() {
        this.Generate(this.rootNode);
    }

    [Button]
    public void Clear() {
        foreach(TechTreeTechnology node in this.nodeGameObjects) {
            if(node != null && node.gameObject != null) {
                GameObject.DestroyImmediate(node.gameObject);
            }
        }
        this.nodeGameObjects.Clear();
    }

    private void CreateGameObjects(List<Technology> allNodes) {
        foreach(var node in allNodes) {
            TechTreeTechnology techTreeNode = GameObject.Instantiate(
                this.nodePrefab,
                new Vector2(node.x, node.y * this.nodeSize.y),
                Quaternion.identity,
                this.nodeInstanceParent).GetComponent<TechTreeTechnology>();
            techTreeNode.gameObject.SetActive(true);
            techTreeNode.SetNode((NodeTechTree)node.graphNode);

            this.nodeGameObjects.Add(techTreeNode);

            node.gameObject = techTreeNode.gameObject;
        }

        foreach(Technology node in allNodes) {
            node.gameObject.GetComponent<TechTreeTechnology>().SetupLine(
                node.parent == null ? null : node.parent.gameObject.transform);
        }
    }

    /// <summary>
    /// Assign final x values to nodes.
    /// </summary>
    private void CalculateFinalPositions(Technology node, float modSum) {
        node.x += modSum;
        modSum += node.Mod;

        foreach(var child in node.children)
            CalculateFinalPositions(child, modSum);

        if(node.children.Count == 0) {
            node.width = node.x;
            node.height = node.y;
        }
        else {
            node.width = node.children.OrderByDescending(p => p.width).First().width;
            node.height = node.children.OrderByDescending(p => p.height).First().height;
        }
    }

    /// <summary>
    /// Calculates the initial x and mod values.
    /// </summary>
    private void CalculateInitialX(Technology node) {
        foreach(var child in node.children)
            CalculateInitialX(child);

        // if no children
        if(node.IsLeaf()) {
            // if there is a previous sibling in this set, set X to prevous sibling + designated distance
            if(!node.IsLeftMost())
                node.x = node.GetPreviousSibling().x + nodeSize.x + siblingDistance;
            else
                // if this is the first node in a set, set X to 0
                node.x = 0;
        }
        // if there is only one child
        else if(node.children.Count == 1) {
            // if this is the first node in a set, set it's X value equal to it's child's X value
            if(node.IsLeftMost()) {
                node.x = node.children[0].x;
            }
            else {
                node.x = node.GetPreviousSibling().x + nodeSize.x + siblingDistance;
                node.Mod = node.x - node.children[0].x;
            }
        }
        else {
            var leftChild = node.GetLeftMostChild();
            var rightChild = node.GetRightMostChild();
            var mid = (leftChild.x + rightChild.x) / 2;

            if(node.IsLeftMost()) {
                node.x = mid;
            }
            else {
                node.x = node.GetPreviousSibling().x + nodeSize.x + siblingDistance;
                node.Mod = node.x - mid;
            }
        }

        if(node.children.Count > 0 && !node.IsLeftMost()) {
            // Since subtrees can overlap, check for conflicts and shift tree right if needed
            CheckForConflicts(node);
        }

    }

    private void CheckForConflicts(Technology node) {
        var minDistance = treeDistance + nodeSize.x;
        var shiftValue = 0F;

        var nodeContour = new Dictionary<int, float>();
        GetLeftContour(node, 0, ref nodeContour);

        var sibling = node.GetLeftMostSibling();
        while(sibling != null && sibling != node) {
            var siblingContour = new Dictionary<int, float>();
            GetRightContour(sibling, 0, ref siblingContour);

            for(int level = node.y + 1; level <= Math.Min(siblingContour.Keys.Max(), nodeContour.Keys.Max()); level++) {
                var distance = nodeContour[level] - siblingContour[level];
                if(distance + shiftValue < minDistance) {
                    shiftValue = minDistance - distance;
                }
            }

            if(shiftValue > 0) {
                node.x += shiftValue;
                node.Mod += shiftValue;

                CenterNodesBetween(node, sibling);

                shiftValue = 0;
            }

            sibling = sibling.GetNextSibling();
        }
    }

    private void CenterNodesBetween(Technology leftNode, Technology rightNode) {
        var leftIndex = leftNode.parent.children.IndexOf(rightNode);
        var rightIndex = leftNode.parent.children.IndexOf(leftNode);

        var numNodesBetween = (rightIndex - leftIndex) - 1;

        if(numNodesBetween > 0) {
            var distanceBetweenNodes = (leftNode.x - rightNode.x) / (numNodesBetween + 1);

            int count = 1;
            for(int i = leftIndex + 1; i < rightIndex; i++) {
                var middleNode = leftNode.parent.children[i];

                var desiredX = rightNode.x + (distanceBetweenNodes * count);
                var offset = desiredX - middleNode.x;
                middleNode.x += offset;
                middleNode.Mod += offset;

                count++;
            }

            CheckForConflicts(leftNode);
        }
    }

    /// <summary>
    /// Ensure no node is being drawn off screen.
    /// </summary>
    private void CheckAllChildrenOnScreen(Technology node) {
        var nodeContour = new Dictionary<int, float>();
        GetLeftContour(node, 0, ref nodeContour);

        float shiftAmount = 0;
        foreach(var y in nodeContour.Keys) {
            if(nodeContour[y] + shiftAmount < 0)
                shiftAmount = (nodeContour[y] * -1);
        }

        if(shiftAmount > 0) {
            node.x += shiftAmount;
            node.Mod += shiftAmount;
        }
    }

    private void GetLeftContour(Technology node, float modSum, ref Dictionary<int, float> values) {
        if(!values.ContainsKey(node.y))
            values.Add(node.y, node.x + modSum);
        else
            values[node.y] = Math.Min(values[node.y], node.x + modSum);

        modSum += node.Mod;
        foreach(var child in node.children) {
            GetLeftContour(child, modSum, ref values);
        }
    }

    private void GetRightContour(Technology node, float modSum, ref Dictionary<int, float> values) {
        if(!values.ContainsKey(node.y))
            values.Add(node.y, node.x + modSum);
        else
            values[node.y] = Math.Max(values[node.y], node.x + modSum);

        modSum += node.Mod;
        foreach(var child in node.children) {
            GetRightContour(child, modSum, ref values);
        }
    }

    public class Technology {

        public float x;
        public int y;
        public float Mod;
        public Technology parent;
        public List<Technology> children;
        public float width;
        public int height;
        public Node graphNode;
        /// <summary>
        /// A reference to the GameObject representing this technology in
        /// the popup ui.
        /// </summary>
        public GameObject gameObject;

        public Technology(Node node, Technology parent, Vector2Int pos) {
            this.graphNode = node;
            this.parent = parent;
            this.children = new List<Technology>();

            this.x = pos.x;
            this.y = pos.y;
        }

        public bool IsLeaf() {
            return this.children.Count == 0;
        }

        public bool IsLeftMost() {
            if(this.parent == null)
                return true;

            return this.parent.children[0] == this;
        }

        public bool IsRightMost() {
            if(this.parent == null)
                return true;

            return this.parent.children[this.parent.children.Count - 1] == this;
        }

        public Technology GetPreviousSibling() {
            if(this.parent == null || this.IsLeftMost())
                return null;

            return this.parent.children[this.parent.children.IndexOf(this) - 1];
        }

        public Technology GetNextSibling() {
            if(this.parent == null || this.IsRightMost())
                return null;

            return this.parent.children[this.parent.children.IndexOf(this) + 1];
        }

        public Technology GetLeftMostSibling() {
            if(this.parent == null)
                return null;

            if(this.IsLeftMost())
                return this;

            return this.parent.children[0];
        }

        public Technology GetLeftMostChild() {
            if(this.children.Count == 0)
                return null;

            return this.children[0];
        }

        public Technology GetRightMostChild() {
            if(this.children.Count == 0)
                return null;

            return this.children[children.Count - 1];
        }

        public override string ToString() {
            return graphNode.ToString();
        }
    }
}