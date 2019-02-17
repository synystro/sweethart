using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public int gridX;
    public int gridY;

    public bool isWall;
    public Vector3 position;

    public Node Parent;

    public int gCost;
    public int hCost;
    int heapIndex;

    public int FCost { get { return gCost + hCost; } }
    public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

    public Node(bool _isWall, Vector3 _position, int _gridX, int _gridY)
    {
        isWall = _isWall;
        position = _position;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int CompareTo(Node nodeToCompare) {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if(compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}