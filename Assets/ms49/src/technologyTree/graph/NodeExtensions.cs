using System.Collections.Generic;
using UnityEngine;
using XNode;

public static class NodeExtensions {

    private static string PARENT = "_parent";
    private static string CHILDREN = "_children";

    public static Node GetParent(this Node node) {
        NodePort parentPort = node.GetPort(PARENT);
        if(parentPort.ConnectionCount == 0) {
            return null; // No parent
        } else {
            return parentPort.GetConnection(0).node;
        }
    }

    public static List<NodePort> GetChildren(this Node node) {
        Debug.Log(node.GetPort(CHILDREN));
        return node.GetPort(CHILDREN).GetConnections();
    }

    private static int GetChildCount(this Node node) {
        return node.GetPort(CHILDREN).ConnectionCount;
    }

    public static int SiblingIndexOf(this Node node) {
        Node parent = node.GetParent();

        if(parent == null) {
            return 0;
        }

        return parent.GetChildren().IndexOf(node.GetPort(PARENT));
    }

    public static bool IsLeaf(this Node node) {
        return node.GetChildCount() == 0;
    }

    public static bool IsLeftMost(this Node node) {
        Node parent = node.GetParent();

        if(parent == null)
            return true;

        return parent.GetChildren()[0].node == node;
    }

    public static bool IsRightMost(this Node node) {
        Node parent = node.GetParent();

        if(parent == null)
            return true;

        return parent.GetChildren()[parent.GetChildCount() - 1].node == node;
    }

    public static Node GetPreviousSibling(this Node node) {
        Node parent = node.GetParent();

        if(parent == null || node.IsLeftMost())
            return null;

        return parent.GetChildren()[node.SiblingIndexOf() - 1].node;
    }

    public static Node GetNextSibling(this Node node) {
        Node parent = node.GetParent();

        if(node.GetParent() == null || node.IsRightMost())
            return null;

        return parent.GetChildren()[node.SiblingIndexOf() + 1].node;
    }

    public static Node GetLeftMostSibling(this Node node) {
        if(node.GetParent() == null)
            return null;

        if(node.IsLeftMost())
            return node;

        return node.GetParent().GetChildren()[0].node;
    }

    public static Node GetLeftMostChild(this Node node) {
        if(node.GetChildCount() == 0)
            return null;

        return node.GetChildren()[0].node;
    }

    public static Node GetRightMostChild(this Node node) {
        if(node.GetChildCount() == 0)
            return null;

        return node.GetChildren()[node.GetChildCount() - 1].node;
    }
}