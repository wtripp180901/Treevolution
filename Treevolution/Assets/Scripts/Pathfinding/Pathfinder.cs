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
        private static (float F, float G, float H)[] scores = new (float F, float G, float H)[] { }; // Stores the F, G, H scores of each node, indexed the same as graph
        private static List<PathfindingNode> graph = new List<PathfindingNode>();

        //TODO: Implement A* pathfinding on pathfindingGraph
        public static Vector3[] GetPath(Vector3 source, Vector3 target)
        {
            if (source == target)
                return new Vector3[] { source };

            UpdatePathfindingGraph(); // Should be called by enemy etc.
            initializeScores(graph.Count, (float.MaxValue, float.MaxValue, float.MaxValue));

            (int srcIndex, PathfindingNode srcNode) = closestNodeToPosition(source);
            (int tgtIndex, PathfindingNode tgtNode) = closestNodeToPosition(target);

            scores[srcIndex] = (Vector3.Distance(srcNode.position, tgtNode.position), 0, Vector3.Distance(srcNode.position, tgtNode.position));

            List<(int index, PathfindingNode node)> open = new List<(int index, PathfindingNode node)> { (srcIndex, srcNode) };
            List<(int index, PathfindingNode node)> closed = new List<(int index, PathfindingNode node)> { };

            while (open.Count > 0)
            {
                (int bestIndex, PathfindingNode bestNode) = getBestNode(open);
                open.Remove((bestIndex, bestNode));
                PathfindingNode[] bestNodeNeighbours = bestNode.neighbours;
                foreach (PathfindingNode neighbour in bestNodeNeighbours)
                {
                    int neighbourIndex = graph.IndexOf(neighbour);
                    (float F, float G, float H) tempScores = (
                        F: 0, 
                        G: scores[bestIndex].G + bestNode.costToNeighbours[neighbour],
                        H: Vector3.Distance(neighbour.position, tgtNode.position));
                    tempScores.F = tempScores.G + tempScores.H;
                    if (closed.Contains((neighbourIndex, neighbour)))
                        continue;
                    else if (!open.Contains((neighbourIndex, neighbour)))
                    {
                        neighbour.parentNode = bestNode;
                        open.Add((neighbourIndex, neighbour));
                        scores[neighbourIndex] = tempScores;
                    }
                    else if (open.Contains((neighbourIndex, neighbour)) && tempScores.F < scores[neighbourIndex].F)
                    {
                        neighbour.parentNode = bestNode;
                        scores[neighbourIndex] = tempScores;
                    }

                }
                closed.Add((bestIndex, bestNode));
            }
            List<Vector3> path = traverseFromTarget(tgtNode);
            if (tgtNode.position != target)
                path.Add(target); // Path to target position, excluding source position
            return path.ToArray();
        }

        private static List<Vector3> traverseFromTarget(PathfindingNode target)
        {
            List<Vector3> positions = new List<Vector3>();
            PathfindingNode currentNode = target;
            while(currentNode != null)
            {
                positions.Insert(0, currentNode.position);
                currentNode = currentNode.parentNode;
            }
            return positions;
        }

        private static (int index, PathfindingNode node) getBestNode(List<(int index, PathfindingNode node)> nodes)
        {
            if (nodes.Count == 0)
                return (-1, null);

            int bestIndex = nodes[0].index;
            foreach((int index, PathfindingNode node) n in nodes)
            {
                if (scores[n.index].F < scores[bestIndex].F)
                {
                    bestIndex = n.index;
                }
            }
            return (bestIndex, graph[bestIndex]);
        }

        private static (int, PathfindingNode) closestNodeToPosition(Vector3 pos)
        {
            (int, float) closestNode = (0, Vector3.Distance(pos, graph[0].position));
            for (int i = 1; i < graph.Count; i++)
            {
                float dist = Vector3.Distance(pos, graph[i].position);
                if (dist < closestNode.Item2)
                {
                    closestNode = (i, dist);
                }
            }
            return (closestNode.Item1, graph[closestNode.Item1]);
        }

        private static void initializeScores(int size, (float, float, float) initialValues)
        {
            scores = new (float F, float G, float H)[size];
            for(int i = 0; i < size; i++)
            {
                scores[i] = initialValues;
            }

        }

        //Should be called whenever a change is made to the position of pathfinding obstacles
        public static void UpdatePathfindingGraph()
        {
            graph = PathfindingGraphGenerator.GetPathfindingGraph().ToList();
        }
    }
}
