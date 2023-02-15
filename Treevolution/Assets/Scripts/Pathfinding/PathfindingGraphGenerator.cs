using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfinding
{
    //Used to dynamically generate a pathfinding mesh based on obstacles in the environment
    public static class PathfindingGraphGenerator
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
            if (GetObstacleDataEvent != null) GetObstacleDataEvent.Invoke(null, null);
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
                        //GameObject marker = GameObject.FindGameObjectWithTag("PlaneMarker");
                        //GameObject.Instantiate(marker, graph[i].position, Quaternion.identity);
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

                    if (pos.x > GameProperties.BottomLeftCorner.x &&
                        pos.x < GameProperties.BottomRightCorner.x &&
                        pos.z > GameProperties.BottomLeftCorner.z &&
                        pos.z < GameProperties.TopLeftCorner.z) withinPlaneBounds = true;

                    if (notInsideTerrain && withinPlaneBounds) graph.Add(new PathfindingNode(i * 10 + j, pos));
                }
            }
            obstacleData.Clear();
            return graph;
        }
    }
}
