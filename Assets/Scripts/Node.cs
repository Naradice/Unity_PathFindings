using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public Node parent;
    public int movementPenalty;

    public int gridX;
    public int gridY;

    // cost from current node to this node
    public int gCost;
    // cost from this node to target node
    public int hCost;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
    }

    public int FCost {
        get{
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare){
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if(compare == 0){
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        // For path finding, we want the lowest FCost to be at the top of the heap
        return -compare;
    }
}
