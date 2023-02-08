using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    //Should be attached to any obstacles which should be avoided during pathfinding
    public class PathfindingObstacle : MonoBehaviour
    {
        Bounds bounds;

        //TODO: move these to central location
        [SerializeField]
        float nodeMargins = 0.1f;
        [SerializeField]
        float floorOffset = 0.1f;

        private void Start()
        {
            bounds = GetComponent<Collider>().bounds;
            PathfindingGraphGenerator.GetObstacleDataEvent += GetObstacleBoundsEventHandler;
        }
        void GetObstacleBoundsEventHandler(object sender, EventArgs args)
        {
            float halfWidth = bounds.extents.x + nodeMargins;
            float halfDepth = bounds.extents.z + nodeMargins;
            Vector3 floorPosition = transform.position - (transform.up * (bounds.extents.y - floorOffset));
            Vector3[] possiblePFPoints = new Vector3[]
            {
            floorPosition + halfWidth * transform.right + halfDepth * transform.forward,
            floorPosition + halfWidth * transform.right - halfDepth * transform.forward,
            floorPosition + -halfWidth * transform.right + halfDepth * transform.forward,
            floorPosition + -halfWidth * transform.right + -halfDepth * transform.forward
            };
            PathfindingGraphGenerator.AddObstacleData(bounds, possiblePFPoints);
        }
    }

}