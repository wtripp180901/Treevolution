using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PathfindingNode
{
    public readonly int cost;
    public readonly Vector3 position;
    public readonly Vector3[] neighbours;

    public PathfindingNode(int cost, Vector3 position, Vector3[] neighbours)
    {
        this.cost = cost;
        this.position = position;
        this.neighbours = neighbours;
    }
}
