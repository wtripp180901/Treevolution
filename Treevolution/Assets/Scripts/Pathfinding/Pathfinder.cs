using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Pathfinding
{
    /// <summary>
    /// Can be utilised by agents who wish to pathfind to get a list of Vector3 coordinates that they can use to navigate through the environment.
    /// </summary>
    public static class Pathfinder
    {
        private static (float F, float G, float H)[] scores = new (float F, float G, float H)[] { }; // Stores the F, G, H scores of each node, indexed the same as graph
        private static List<PathfindingNode> graph = new List<PathfindingNode>();

        /// <summary>
        /// A* Implementation to calculate the shortest path through the generated graph.
        /// Since the graph generated includes line-of-sight calculations, this algorithm closely resembles Theta*
        /// </summary>
        /// <param name="source">Source location (of the agent).</param>
        /// <param name="target">Target location (of the destination).</param>
        /// <returns>Array of positions to which the agent can follow to reach the target destination.</returns>
        public static Vector3[] GetPath(Vector3 source, Vector3 target,bool avoidWalls)
        {
            if (avoidWalls)
            {
                UpdatePathfindingGraph(); // Updates the local graph to correspond to the environment (e.g. obstacles).
                initializeScores(graph.Count, (float.MaxValue, float.MaxValue, float.MaxValue)); // Initialises all the score values in the FGH array.


                if (source == target || graph.Count == 0 || !Physics.Raycast(source, target - source, (target - source).magnitude, 1 << 3))
                    return new Vector3[] { target }; // If the agent is already at the target, or no obstacles are in the environment, then return the target.


                    (int srcIndex, PathfindingNode srcNode) = closestNodeToPosition(source); // Calculate the closest graph node to the source position.
                (int tgtIndex, PathfindingNode tgtNode) = closestNodeToPositionFromDirection(target, source); // Calculate the closest node to the target from the direction of the source.

                scores[srcIndex] = (Vector3.Distance(srcNode.position, tgtNode.position), 0, Vector3.Distance(srcNode.position, tgtNode.position));

                List<(int index, PathfindingNode node)> open = new List<(int index, PathfindingNode node)> { (srcIndex, srcNode) }; // List of open nodes
                List<(int index, PathfindingNode node)> closed = new List<(int index, PathfindingNode node)> { }; // List of closed nodes

                while (open.Count > 0) // Whilst there are still nodes to consider ...
                {
                    (int bestIndex, PathfindingNode bestNode) = getBestNode(open); // Calculate the best node in the open list
                    open.Remove((bestIndex, bestNode)); // Close the node
                    PathfindingNode[] bestNodeNeighbours = bestNode.neighbours;
                    foreach (PathfindingNode neighbour in bestNodeNeighbours) // Iterate over it's neighbours
                    {
                        int neighbourIndex = graph.IndexOf(neighbour);
                        (float F, float G, float H) tempScores = (
                            F: 0,
                            G: scores[bestIndex].G + bestNode.costToNeighbours[neighbour],
                            H: Vector3.Distance(neighbour.position, tgtNode.position));
                        tempScores.F = tempScores.G + tempScores.H;// Calculate temporary F, G, and H scores of the neighbour
                        if (closed.Contains((neighbourIndex, neighbour))) // If the neighbour is already closed then ignore it
                            continue;
                        else if (!open.Contains((neighbourIndex, neighbour))) // Otherwise, if the neighbour isn't already open ...
                        {
                            neighbour.parentNode = bestNode; // Update the neighbours parent
                            open.Add((neighbourIndex, neighbour)); // Open the neighbour
                            scores[neighbourIndex] = tempScores; // Assign the neighbour's score
                        }
                        else if (open.Contains((neighbourIndex, neighbour)) && tempScores.F < scores[neighbourIndex].F) // Otherwise, if the neighbour is already open and the current temporary score is an improvement to before...
                        {
                            neighbour.parentNode = bestNode; // Update the neighbour's parent
                            scores[neighbourIndex] = tempScores; // Update the neighbour's score
                        }

                    }
                    closed.Add((bestIndex, bestNode));
                }
                List<Vector3> path = traverseFromTarget(tgtNode);
                if (tgtNode.position != target)
                    path.Add(target); // Path to target position, excluding source position
                return path.ToArray();
            }
            else
            {
                return new Vector3[] { source, target };
            }
        }


        private static List<Vector3> traverseFromTarget(PathfindingNode target)
        {
            List<Vector3> positions = new List<Vector3>();
            PathfindingNode currentNode = target;
            while (currentNode != null)
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
            foreach ((int index, PathfindingNode node) n in nodes)
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
            (int index, float distToTarget) closestNode = (0, Vector3.Distance(pos, graph[0].position));
            for (int i = 1; i < graph.Count; i++)
            {
                float dist = Vector3.Distance(pos, graph[i].position);
                if (dist < closestNode.distToTarget)
                {
                    closestNode = (i, dist);
                }
            }
            return (closestNode.index, graph[closestNode.index]);
        }

        private static (int, PathfindingNode) closestNodeToPositionFromDirection(Vector3 pos, Vector3 fromPos)
        {
            (int index, float distToTarget, float distFromDir) closestNodeFromDirection = (0, Vector3.Distance(pos, graph[0].position), Vector3.Distance(fromPos, graph[0].position));
            for (int i = 1; i < graph.Count; i++)
            {
                float dist = Vector3.Distance(pos, graph[i].position);
                float distFromDir = Vector3.Distance(fromPos, graph[i].position);

                if (dist < closestNodeFromDirection.distToTarget)
                {
                    closestNodeFromDirection = (i, dist, distFromDir);
                }
                else if (dist == closestNodeFromDirection.distToTarget && distFromDir < closestNodeFromDirection.distFromDir)
                {
                    closestNodeFromDirection = (i, dist, distFromDir);
                }
            }
            return (closestNodeFromDirection.index, graph[closestNodeFromDirection.index]);
        }

        private static void initializeScores(int size, (float, float, float) initialValues)
        {
            scores = new (float F, float G, float H)[size];
            for (int i = 0; i < size; i++)
            {
                scores[i] = initialValues;
            }

        }

        /// <summary>
        /// Updates the Pathfinder's graph of nodes to reflect the current obstacles in the environment.
        /// </summary>
        public static void UpdatePathfindingGraph()
        {
            graph = PathfindingGraphGenerator.GetPathfindingGraph().ToList();
        }
    }
}