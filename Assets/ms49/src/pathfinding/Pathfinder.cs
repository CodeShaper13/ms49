using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder {

    public const int MAX_NAV_COUNT = 15;

    private static Heap<PathfindingNode> openSet;
    private static NavMap[] maps = new NavMap[MAX_NAV_COUNT];

    public static bool IsInitialized => openSet != null;

    public static void Initialize(int openSetSize) {
        openSet = new Heap<PathfindingNode>(openSetSize);
    }

    public static void RegisterNavMap(NavMap map) {
        if(maps[map.NavId] != null) {
            Debug.LogWarning(
                "NavMap already registered with an ID of " + map.NavId,
                map.gameObject);
        }

        maps[map.NavId] = map;
    }

    public static void UnregisterNavMap(NavMap map) {
        maps[map.NavId] = null;
    }

    public static Position[] FindPath(int navMapId, Position start, Position end, bool stopAdjacentToFinish) {
        NavMap navMap = maps[navMapId];

        if(navMap == null) {
            Debug.LogWarningFormat("NavMapId of {0} is not registered", navMapId);
            return null;
        }
        
        PathfindingNode startNode = navMap.GetNode(start);
        PathfindingNode endNode = navMap.GetNode(end);

        openSet.Clear();
        HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();
        openSet.Add(startNode);

        PathfindingNode[] cachedNeighborArray = new PathfindingNode[7];

        while(openSet.count > 0) {
            PathfindingNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode == endNode) {
                return RetracePath(startNode, endNode, stopAdjacentToFinish);
            }

            navMap.GetAdjacentNodes(currentNode, cachedNeighborArray);
            foreach(PathfindingNode neighbor in cachedNeighborArray) {
                if(neighbor == null) {
                    break; // Reached the "end" of the array
                }

                if(closedSet.Contains(neighbor)) {
                    continue; // already visited this node.
                }

                if(!neighbor.IsWalkable && neighbor != endNode) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;
                if(newMovementCostToNeighbour < neighbor.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = newMovementCostToNeighbour;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if(!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    }
                    else {
                        openSet.UpdateItem(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private static int GetDistance(PathfindingNode nodeA, PathfindingNode nodeB) {

        int distX = Math.Abs(nodeA.x - nodeB.x);
        int distZ = Math.Abs(nodeA.depth - nodeB.depth);
        int distY = Math.Abs(nodeA.y - nodeB.y);

        if(distX > distZ) {
            return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
        }
        else {
            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }
    }

    private static Position[] RetracePath(PathfindingNode startNode, PathfindingNode endNode, bool stopAdjacentToFinish) {
        if(startNode == endNode) {
            return null;
        }

        List<PathfindingNode> path = new List<PathfindingNode>();
        PathfindingNode currentNode = stopAdjacentToFinish ? endNode.parent : endNode;

        while(currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        if(path.Count == 0) {
            // The path has a length of 1 (destination is an adjacent cell) and stopAdjacentToFinish is true.
            return new Position[] { startNode.Position };
        }

        // Convert the List of Nodes into an Array of PathPoints.
        Position[] waypoints = new Position[path.Count];
        for(int i = 0; i < path.Count; i++) {
            waypoints[i] = path[i].Position;
        }
        Array.Reverse(waypoints);


        return waypoints;
    }
}