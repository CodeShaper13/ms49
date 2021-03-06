﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class NavGrid {

    public Node[,] nodes;

    private int mapSize;

    public NavGrid(int size) {
        this.nodes = new Node[size, size];
        this.mapSize = size;
    }

    /// <summary>
    /// Returns the node at the passed position.
    /// </summary>
    public Node getNode(Vector2 pos) {
        return nodes[(int)pos.x, (int)pos.y];
    }

    /// <summary>
    /// Returns a List of all adjacent nodes.
    /// </summary>
    public List<Node> getAdjacentNodes(Node node) {
        List<Node> neighbours = new List<Node>();
        
        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                if(x == 0 && y == 0) {
                    continue;
                }

                // Prevent diagonals from being added
                if(Math.Abs(x) == Math.Abs(y)) {
                    continue;
                }

                int checkX = node.x + x;
                int checkY = node.y + y;

                if(this.inMap(checkX, checkY)) {
                    neighbours.Add(nodes[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public bool inMap(int x, int y) {
        return x >= 0 && x < this.mapSize && y >= 0 && y < this.mapSize;
    }

    public void debugDraw() {
        Gizmos.color = Color.green;

        Grid g = GameObject.FindObjectOfType<Grid>();

        foreach(Node n in this.nodes) {
            if(n.isWalkable) {
                Gizmos.DrawSphere(n.worldPosition, 0.25f);
            }
        }
    }
}
