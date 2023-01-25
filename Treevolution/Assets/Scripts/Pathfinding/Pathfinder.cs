using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    //TODO: Implement A* pathfinding
    public static Vector3[] GetPath(Vector3 source,Vector3 target,PathfindingNode[] pathfindingGraph)
    {
        return new Vector3[] { new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(2, 1, 0) }; //placeholder
    }
}
