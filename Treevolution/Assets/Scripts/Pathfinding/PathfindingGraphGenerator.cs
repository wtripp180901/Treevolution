using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>
    /// Used to dynamically generate a pathfinding mesh based on obstacles in the environment
    /// </summary>
    public static class PathfindingGraphGenerator
    {

        public struct ObstacleData
        {
            public readonly Bounds bounds;
            public readonly Vector3[] possiblePFPoints;

            public ObstacleData(Bounds bounds, Vector3[] possiblePFPoints)
            {
                this.bounds = bounds;
                this.possiblePFPoints = possiblePFPoints;
            }
        }

        /// <summary>
        /// Should be subscribed to by any obstacles, when received should send their pathfinding points to this class via AddObstacleData
        /// </summary>
        public static event EventHandler<EventArgs> GetObstacleDataEvent;

        /// <summary>
        /// List of raw obstacle data
        /// </summary>
        private static List<ObstacleData> obstacleData = new List<ObstacleData>();

        /// <summary>
        /// Callback method for when GetObstacleDataEvent is invoked, used to add an obstacles pathfinding points for consideration
        /// </summary>
        /// <param name="bounds">The bounds of an obstacle's collider</param>
        /// <param name="possiblePoints">An obstacles pathfinding points for consideration</param>
		/// <returns></returns>
        public static ObstacleData AddObstacleData(Bounds bounds, Vector3[] possiblePoints)
        {
            ObstacleData newObstacle = new ObstacleData(bounds, possiblePoints);
            obstacleData.Add(newObstacle);
            return newObstacle;
        }

        public static void ClearObstacleData()
        {
            obstacleData.Clear();
        }

        public static void RemoveFromObstacleData(ObstacleData dataToRemove)
        {
            obstacleData.Remove(dataToRemove);
        }


        /// <summary>
        /// Generates pathfinding graph based on current obstacles in the environment
        /// </summary>
        /// <returns>List of pathfining nodes based on current state of environment</returns>
        public static PathfindingNode[] GetPathfindingGraph()
        {
            if (GetObstacleDataEvent != null) GetObstacleDataEvent.Invoke(null, null);
            List<PathfindingNode> graph = new List<PathfindingNode>();
            // Graph[0] = tree
            graph.Add(new PathfindingNode(-1, GameObject.FindWithTag("Tree").transform.position));
            graph.AddRange(nodesFromObstacleData());
            
            for (int i = 0; i < graph.Count; i++)
            {
                for (int j = i + 1; j < graph.Count; j++)
                {
                    Vector3 directionRay = graph[j].position - graph[i].position;
                    float distance = directionRay.magnitude;

                    //Mask for the ObstaclesOnly layer, rewrite this to use layer from inspector rather than hardcoding
                    int layerMask = 1 << 3;

                    if (!Physics.Raycast(graph[i].position, directionRay, distance, layerMask))
                    {
                        if (i == 0)
                            distance = 0;
                        graph[i].AddNeighbour(graph[j], distance);
                        graph[j].AddNeighbour(graph[i], distance);
                        Debug.DrawLine(graph[i].position, graph[i].position + directionRay, Color.red, 3);

                    }
                }
            }
            return graph.ToArray();
        }

        /// <summary>
        /// Helper method for GetPathfindingGraph, iterates through obstacleData (set in GetPathfindingGraph) and creates nodes from the data if they are within the bounds of the board and aren't inside other objects geometry
        /// </summary>
        /// <returns>Points which meet the criteria to be nodes used in the pathfinding graph</returns>
        private static List<PathfindingNode> nodesFromObstacleData()
        {

            List<PathfindingNode> graph = new List<PathfindingNode>();
            for (int i = 0; i < obstacleData.Count; i++)
            {
                for (int j = 0; j < obstacleData[i].possiblePFPoints.Length; j++)
                {
                    bool notInsideTerrain = true;
                    bool withinPlaneBounds = false;

                    Vector3 pos = obstacleData[i].possiblePFPoints[j];

                    for (int k = 0; k < obstacleData.Count; k++)
                    {
                        if (obstacleData[k].bounds.Contains(pos))
                        {
                            notInsideTerrain = false;
                            break;
                        }
                    }


                    // Checks if point lies within the rectangle from a top-down view
                    Vector2 pos2D = new Vector2(pos.x, pos.z);
                    Vector2 a = new Vector2(GameProperties.TopLeftCorner.x, GameProperties.TopLeftCorner.z);
                    Vector2 b = new Vector2(GameProperties.TopRightCorner.x, GameProperties.TopRightCorner.z);
                    Vector2 d = new Vector2(GameProperties.BottomLeftCorner.x, GameProperties.BottomLeftCorner.z);
                    if (0 < Vector2.Dot((pos2D - a), (b - a)) && Vector2.Dot((pos2D - a), (b - a)) < Vector2.Dot((b - a), (b - a)))
                    {
                        if (0 < Vector2.Dot((pos2D - a), (d - a)) && Vector2.Dot((pos2D - a), (d - a)) < Vector2.Dot((d - a), (d - a)))
                        {
                            withinPlaneBounds = true;
                        }
                    }
                    if (notInsideTerrain && withinPlaneBounds) graph.Add(new PathfindingNode(i * 10 + j, pos));
                    else
                    {
                        Debug.DrawLine(pos, pos + Vector3.up * 5, Color.red, 1000);
                    }
                }
            }
            obstacleData.Clear();
            return graph;
        }
    }
}