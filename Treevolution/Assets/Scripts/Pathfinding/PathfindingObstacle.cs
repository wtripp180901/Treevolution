using System;
using UnityEngine;

namespace Pathfinding
{   
    /// <summary>
    /// Script to be attached to any object which enemies will pathfind around during the game
    /// </summary>
    public class PathfindingObstacle : MonoBehaviour
    {
        //TODO: move these to central location
        [SerializeField]
        float nodeMargins = 0.1f;
        [SerializeField]
        float floorOffset = 0.1f;
        Bounds bounds;
        BoxCollider myCollider;

        /// <summary>
        /// Determines if the PathfindingObstacle will send nodes to PathfindingGraphGenerator
        /// </summary>
        bool sendNodes = true;

        public static Vector3[] CalculateCorners(Vector3 centreFloor, float extentsWidth, Vector3 widthVec, float extentsDepth, Vector3 depthVec)
        {
            Vector3[] offsetCorners = new Vector3[]
            {
                centreFloor + extentsWidth * widthVec + extentsDepth * depthVec,
                centreFloor + extentsWidth * widthVec - extentsDepth * depthVec,
                centreFloor - extentsWidth * widthVec + extentsDepth * depthVec,
                centreFloor - extentsWidth * widthVec - extentsDepth * depthVec
            };
            return offsetCorners;
        }

        private void Start()
        {
            myCollider = gameObject.GetComponent<BoxCollider>();
            bounds = myCollider.bounds;
            PathfindingGraphGenerator.GetObstacleDataEvent += GetObstacleBoundsEventHandler;
        }
        void GetObstacleBoundsEventHandler(object sender, EventArgs args)
        {
            if (sendNodes)
            {
                float halfWidthWithMargin = bounds.size.x / 2 + nodeMargins;
                float halfDepthWithMargin = bounds.size.z / 2 + nodeMargins;
                float halfHeightExact = bounds.size.y / 2;

                Vector3 centre = transform.position;
                Vector3 floorPosition = centre - (myCollider.transform.up * (halfHeightExact - floorOffset));
                Debug.DrawLine(floorPosition, floorPosition + 20 * myCollider.transform.up, Color.magenta, 100);


                Debug.DrawLine(floorPosition, floorPosition + halfWidthWithMargin * myCollider.transform.right, Color.blue, 100);
                Debug.DrawLine(floorPosition, floorPosition + halfDepthWithMargin * myCollider.transform.forward, Color.blue, 100);
                Vector3[] possiblePFPoints = CalculateCorners(floorPosition, halfWidthWithMargin, myCollider.transform.right, halfDepthWithMargin, myCollider.transform.forward);
                Debug.DrawLine(possiblePFPoints[0], possiblePFPoints[0] + Vector3.up, Color.green, 1000);
                Debug.DrawLine(possiblePFPoints[1], possiblePFPoints[1] + Vector3.up, Color.green, 1000);
                Debug.DrawLine(possiblePFPoints[2], possiblePFPoints[2] + Vector3.up, Color.green, 1000);
                Debug.DrawLine(possiblePFPoints[3], possiblePFPoints[3] + Vector3.up, Color.green, 1000);

                PathfindingGraphGenerator.AddObstacleData(bounds, possiblePFPoints);
            }
        }

        public void SetSendsNodes(bool sends)
        {
            sendNodes = sends;
            Pathfinding.PathfindingUpdatePublisher.NotifyObstacleChanged();
        }
    }

}