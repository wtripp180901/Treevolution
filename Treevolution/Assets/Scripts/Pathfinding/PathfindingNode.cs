using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{

    public class PathfindingNode
    {
        public readonly int id;
        public readonly Vector3 position;
        private List<PathfindingNode> _neighbours = new List<PathfindingNode>();
        public PathfindingNode parentNode { get; set; }
        public float gScore {get; set;} = 0; // Cumulative score from source
        public float hScore { get; set; } = float.MaxValue; // Heuristic to target
        public float fScore { get { return hScore + gScore;  } }

        public PathfindingNode[] neighbours { get { return _neighbours.ToArray(); } }
        public readonly Dictionary<PathfindingNode, float> costToNeighbours = new Dictionary<PathfindingNode, float>();

        public PathfindingNode(int id, Vector3 position)
        {
            this.position = position;
            this.id = id;
        }

        public void AddNeighbour(PathfindingNode neighbour, float cost)
        {
            _neighbours.Add(neighbour);
            costToNeighbours.Add(neighbour, cost);
        }


    }

}
