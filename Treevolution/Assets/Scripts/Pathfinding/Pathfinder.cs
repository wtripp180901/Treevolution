using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Pathfinding
{
    //Can be called to by agents to get a list of Vector3 coordinates to navigate through the environment
    public static class Pathfinder
    {
        private static PathfindingNode[] pathfindingGraph = new PathfindingNode[] { };

        //TODO: Implement A* pathfinding on pathfindingGraph
        public static Vector3[] GetPath(Vector3 source, Vector3 target)
        {
            PathfindingNode sourceNode = new PathfindingNode(0, source);
            bool targetFound = false;

            sourceNode.hScore = Vector3.Distance(source, target);
            List<PathfindingNode> open = new List<PathfindingNode> { sourceNode };
            List<PathfindingNode> closed = new List<PathfindingNode>();
            while (!targetFound || open.Count == 0)
            {
                PathfindingNode bestNode = getBestNode(open);
                if (bestNode.position == target)
                {
                    targetFound = true;
                }
                closed.Add(bestNode);
                open.Remove(bestNode);
                PathfindingNode[] bestNodeNeighbours = bestNode.neighbours;
                for (int i = 0; i < bestNodeNeighbours.Length; i++)
                {
                    if (closed.Contains(bestNodeNeighbours[i]))
                    {
                        continue;
                    }
                    else if (!open.Contains(bestNodeNeighbours[i]))
                    {
                        bestNodeNeighbours[i] = calculateChildScores(bestNodeNeighbours[i], bestNode, target);
                        open.Add(bestNodeNeighbours[i]);
                    }
                    else if (open.Contains(bestNodeNeighbours[i]) && (bestNodeNeighbours[i].gScore < (bestNode.gScore + bestNode.costToNeighbours[bestNodeNeighbours[i]])))
                    {
                        bestNodeNeighbours[i] = calculateChildScores(bestNode, bestNodeNeighbours[i], target);
                    }
                }
            }
            List<PathfindingNode> path; // Traverse Path from Target;
            return new Vector3[] { GameObject.FindGameObjectWithTag("Tree").transform.position }; //placeholder
        }

        private static PathfindingNode calculateChildScores(PathfindingNode parentNode, PathfindingNode childNode, Vector3 target)
        {
            childNode.gScore = parentNode.gScore + parentNode.costToNeighbours[childNode];
            childNode.hScore = Vector3.Distance(childNode.position, target);
            childNode.parentNode = parentNode;
            return childNode;
        }

        private static PathfindingNode getBestNode(List<PathfindingNode> nodes)
        {
            (int, float) lowestScoringNode = (0, nodes[0].fScore);
            for(int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].fScore < lowestScoringNode.Item2)
                {
                    lowestScoringNode = (i, nodes[i].fScore);
                }
            }
            return nodes[lowestScoringNode.Item1];
        }

        //Should be called whenever a change is made to the position of pathfinding obstacles
        public static void UpdatePathfindingGraph()
        {
            pathfindingGraph = PathfindingGraphGenerator.GetPathfindingGraph();
        }
    }
}
