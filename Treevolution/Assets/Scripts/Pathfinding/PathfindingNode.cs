using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>
    /// A node used as part of the A* pathfinding algorithm
    /// </summary>
    public class PathfindingNode
    {
        /// <summary>
        /// A unique identifier for the node
        /// </summary>
        public readonly int id;
        /// <summary>
        /// The nodes real world position
        /// </summary>
        public readonly Vector3 position;

        private List<PathfindingNode> _neighbours = new List<PathfindingNode>();
        /// <summary>
        /// The previous node in the searched path
        /// </summary>
        public PathfindingNode parentNode { get; set; }

        /// <summary>
        /// An array of nodes with a line of sight to this node
        /// </summary>
        public PathfindingNode[] neighbours { get { return _neighbours.ToArray(); } }
        /// <summary>
        /// The cost of moving to a neighbouring node
        /// </summary>
        public readonly Dictionary<PathfindingNode, float> costToNeighbours = new Dictionary<PathfindingNode, float>();

        public PathfindingNode(int id, Vector3 position)
        {
            float floorHeight = GameProperties.FloorHeight;
            position.y = floorHeight;
            this.position = position;
            this.id = id;
        }

        /// <summary>
        /// Adds a neighbour to the node
        /// </summary>
        /// <param name="neighbour">A node with a line of sight to this node</param>
        /// <param name="cost">The cost of moving from this node to the neighbouring node</param>
        public void AddNeighbour(PathfindingNode neighbour, float cost)
        {
            _neighbours.Add(neighbour);
            costToNeighbours.Add(neighbour, cost);
        }


    }

}
