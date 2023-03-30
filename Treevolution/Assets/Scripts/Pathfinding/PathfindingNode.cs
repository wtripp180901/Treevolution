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

        public PathfindingNode[] neighbours { get { return _neighbours.ToArray(); } }
        public readonly Dictionary<PathfindingNode, float> costToNeighbours = new Dictionary<PathfindingNode, float>();

        public PathfindingNode(int id, Vector3 position)
        {
            float floorHeight = GameProperties.FloorHeight;
            position.y = floorHeight;
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
