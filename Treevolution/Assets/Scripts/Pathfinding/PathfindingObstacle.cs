using System;
using UnityEngine;
using static Pathfinding.PathfindingGraphGenerator;

namespace Pathfinding
{   
    /// <summary>
    /// Script to be attached to any object which enemies will pathfind around during the game
    /// </summary>
    public class PathfindingObstacle : MonoBehaviour
    {
        /// <summary>
        /// The margin from the corners of the object's collider its node should be spawned at
        /// </summary>
        [SerializeField]
        float nodeMargins = 0.1f;
        /// <summary>
        /// The height from the floor the nodes should be spawned at
        /// </summary>
        [SerializeField]
        float floorOffset = 0.1f;
        Bounds bounds;
        BoxCollider myCollider;
        private Vector3[] _possiblePFPoints;
        private ObstacleData _obstacleData;

        /// <summary>
        /// Determines if the PathfindingObstacle will send nodes to PathfindingGraphGenerator
        /// </summary>
        bool sendNodes = true;

        /// <summary>
        /// Returns the corners of the collider
        /// </summary>
        /// <param name="centreFloor">The centre of the collider</param>
        /// <param name="extentsWidth">The width of the collider's bounds.extents</param>
        /// <param name="widthVec">The widthwise vector of the object</param>
        /// <param name="extentsDepth">The depth of the collider's bounds.extents</param>
        /// <param name="depthVec">The depthwise vector of the object</param>
        /// <returns>An array of the corners of the object given its current position and orientation</returns>
        public static Vector3[] CalculateCorners(Vector3 centreFloor, float extentsWidth, Vector3 widthVec, float extentsDepth, Vector3 depthVec)
        {
            Vector3[] offsetCorners = new Vector3[]
            {
                (centreFloor - extentsWidth * widthVec - extentsDepth * depthVec),
                centreFloor + extentsWidth * widthVec - extentsDepth * depthVec,
                centreFloor + extentsWidth * widthVec + extentsDepth * depthVec,
                centreFloor - extentsWidth * widthVec + extentsDepth * depthVec
            };
            return offsetCorners;
        }

        private void Start()
        {
            myCollider = gameObject.GetComponent<BoxCollider>();
            bounds = myCollider.bounds;
            GetObstacleDataEvent += GetObstacleBoundsEventHandler;
        }

        /// <summary>
        /// Used as an event handler for GetObstacleDataEvent. Adds this obstacle's nodes to PathfindingGraphGenerator for consideration
        /// </summary>
        /// <param name="sender">Unused, can be null</param>
        /// <param name="args">Unused, can be null</param>
        void GetObstacleBoundsEventHandler(object sender, EventArgs args)
        {
            if (sendNodes)
            {
                float halfWidthWithMargin = myCollider.size.x / 2 + nodeMargins;
                float halfDepthWithMargin = myCollider.size.z / 2 + nodeMargins;
                float halfHeightExact = myCollider.size.y / 2;

                Vector3 centre = transform.position;
                Vector3 floorPosition = new Vector3(centre.x, GameProperties.FloorHeight + floorOffset, centre.z);
                //Debug.DrawLine(floorPosition, floorPosition + 20 * myCollider.transform.up, Color.magenta, 100);


                //Debug.DrawLine(floorPosition, floorPosition + halfWidthWithMargin * myCollider.transform.right, Color.blue, 100);
                //Debug.DrawLine(floorPosition, floorPosition + halfDepthWithMargin * myCollider.transform.forward, Color.blue, 100);
                _possiblePFPoints = CalculateCorners(floorPosition, halfWidthWithMargin, myCollider.transform.right, halfDepthWithMargin, myCollider.transform.forward);
                Debug.DrawLine(_possiblePFPoints[0], _possiblePFPoints[0] + Vector3.up * 0.03f, Color.green, 2);
                Debug.DrawLine(_possiblePFPoints[1], _possiblePFPoints[1] + Vector3.up * 0.03f, Color.green, 2);
                Debug.DrawLine(_possiblePFPoints[2], _possiblePFPoints[2] + Vector3.up * 0.03f, Color.green, 2);
                Debug.DrawLine(_possiblePFPoints[3], _possiblePFPoints[3] + Vector3.up * 0.03f, Color.green, 2);

                _obstacleData = AddObstacleData(bounds, _possiblePFPoints);
            }
        }

        /// <summary>
        /// Enables or disables this object being considered for pathfinding and notifies PathfindingUpdatePublisher that the pathfinding environment has changed
        /// </summary>
        /// <param name="sends">If true, should be considered for pathfinding</param>
        public void SetSendsNodes(bool sends)
        {
            sendNodes = sends;
            PathfindingUpdatePublisher.NotifyObstacleChanged();
        }

        public void CleanForTest()
        {
            GetObstacleDataEvent -= GetObstacleBoundsEventHandler;
        }

        void OnDestroy()
        {
            RemoveFromObstacleData(_obstacleData);
            CleanForTest();
        }
    }

}