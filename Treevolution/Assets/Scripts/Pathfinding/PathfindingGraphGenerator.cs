using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfinding
{
    //Used to dynamically generate a pathfinding mesh based on obstacles in the environment
    static class PathfindingGraphGenerator
    {
        struct ObstacleData
        {
            public readonly Bounds bounds;
            public readonly Vector3[] possiblePFPoints;

            public ObstacleData(Bounds bounds, Vector3[] possiblePFPoints)
            {
                this.bounds = bounds;
                this.possiblePFPoints = possiblePFPoints;
            }
        }

        public static event EventHandler<EventArgs> GetObstacleDataEvent;

        private static List<ObstacleData> obstacleData = new List<ObstacleData>();

        public static void AddObstacleData(Bounds bounds, Vector3[] possiblePoints)
        {
            obstacleData.Add(new ObstacleData(bounds, possiblePoints));
        }

        public static PathfindingNode[] GetPathfindingGraph()
        {
            GetObstacleDataEvent.Invoke(null, null);
            List<PathfindingNode> graph = nodesFromObstacleData();
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
                        graph[i].AddNeighbour(graph[j], distance);
                        graph[j].AddNeighbour(graph[i], distance);
                        Debug.DrawLine(graph[i].position, graph[i].position + directionRay, Color.red, 60);
                    }
                }
            }
            return graph.ToArray();
        }

        private static List<PathfindingNode> nodesFromObstacleData()
        {
            List<PathfindingNode> graph = new List<PathfindingNode>();
            for (int i = 0; i < obstacleData.Count; i++)
            {
                for (int j = 0; j < obstacleData[i].possiblePFPoints.Length; j++)
                {
                    bool notInsideTerrain = true;
                    for (int k = 0; k < obstacleData.Count; k++)
                    {
                        if (obstacleData[k].bounds.Contains(obstacleData[i].possiblePFPoints[j]))
                        {
                            notInsideTerrain = false;
                            break;
                        }
                    }
                    if (notInsideTerrain) graph.Add(new PathfindingNode(i * 10 + j, obstacleData[i].possiblePFPoints[j]));
                }
            }
            obstacleData.Clear();
            return graph;
        }
    }
}
