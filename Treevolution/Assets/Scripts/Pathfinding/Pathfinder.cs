using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    //Can be called to by agents to get a list of Vector3 coordinates to navigate through the environment
    public static class Pathfinder
    {
        private static PathfindingNode[] pathfindingGraph = new PathfindingNode[] { };

        //TODO: Implement A* pathfinding on pathfindingGraph
        public static Vector3[] GetPath(Vector3 source, Vector3 target)
        {
            return new Vector3[] { GameObject.FindGameObjectWithTag("Tree").transform.position }; //placeholder
        }

        //Should be called whenever a change is made to the position of pathfinding obstacles
        public static void UpdatePathfindingGraph()
        {
            pathfindingGraph = PathfindingGraphGenerator.GetPathfindingGraph();
        }
    }
}
