using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfinding
{
    //Should be attached to any obstacles which should be avoided during pathfinding
    public class PathfindingObstacle : MonoBehaviour
    {
        //TODO: move these to central location
        [SerializeField]
        float nodeMargins = 0.1f;
        [SerializeField]
        float floorOffset = 0.1f;
        Bounds bounds;
        Collider myCollider;

        private void Start()
        {
            myCollider = gameObject.GetComponent<Collider>();
            bounds = myCollider.bounds;
            PathfindingGraphGenerator.GetObstacleDataEvent += GetObstacleBoundsEventHandler;
        }
        void GetObstacleBoundsEventHandler(object sender, EventArgs args)
        {
            //GameObject marker = GameObject.FindGameObjectWithTag("PlaneMarker");
            float halfWidth = myCollider.transform.lossyScale.x / 2 + nodeMargins;
            float halfDepth = myCollider.transform.lossyScale.z / 2 + nodeMargins;
            //Instantiate(marker, bounds.center + halfWidth * myCollider.transform.right, Quaternion.identity);
            //Instantiate(marker, bounds.center + halfDepth * myCollider.transform.forward, Quaternion.identity);

            Vector3 floorPosition = bounds.center - (myCollider.transform.up * (bounds.extents.y - floorOffset));
            Debug.DrawLine(bounds.center, bounds.center + halfWidth * myCollider.transform.right, Color.black, 100);
            Debug.DrawLine(bounds.center, bounds.center + halfDepth * myCollider.transform.forward, Color.black, 100);
            Vector3[] possiblePFPoints = new Vector3[]
            {
            floorPosition + halfWidth * myCollider.transform.right + halfDepth * myCollider.transform.forward,
            floorPosition + halfWidth * myCollider.transform.right - halfDepth * myCollider.transform.forward,
            floorPosition - halfWidth * myCollider.transform.right + halfDepth * myCollider.transform.forward,
            floorPosition - halfWidth * myCollider.transform.right - halfDepth * myCollider.transform.forward
            };
            PathfindingGraphGenerator.AddObstacleData(bounds, possiblePFPoints);
        }
    }

}