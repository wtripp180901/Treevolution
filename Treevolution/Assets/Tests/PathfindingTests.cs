using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PathfindingTests
    {
        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator PathfinderWorksWithoutGraph()
        {
            GameObject tree = new GameObject();
            tree.tag = "Tree";
            //tree.AddComponent<Transform>();
            tree.transform.position = new Vector3(0, 0, 0);
            //Assert.AreEqual(Pathfinding.PathfindingGraphGenerator.GetPathfindingGraph().Length, 0,"error");
            Assert.AreEqual(Pathfinding.Pathfinder.GetPath(new Vector3(0, 0, 0), new Vector3(0, 0, 0), true).Length, 1);
            yield return null;
        }

        [Test]
        public void PathfindingGraphGeneratorWorksWithoutObstacles()
        {
            GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tree.tag = "Tree";
            GameObject.Instantiate(tree);
            Assert.AreEqual(1, Pathfinding.PathfindingGraphGenerator.GetPathfindingGraph().Length);
        }

        [Test]
        public void PathfindingGraphGeneratorWorksWithObstacles()
        {
            GameObject logic = new GameObject();
            PlaneMapper pm = logic.AddComponent<PlaneMapper>();
            pm.SetupForTest(true);
            pm.floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            pm.planeMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tree.tag = "Tree";
            pm.treeModel = tree;

            GameObject bird = new GameObject();
            bird.tag = "Buddy";

            pm.tableWidth = 1000;
            pm.tableDepth = 1000;
            pm.CreateNewPlane(Pose.identity);
            RealWorldPropertyMapper rwpm = logic.AddComponent<RealWorldPropertyMapper>();
            rwpm.SetupForTest(pm);
            rwpm.MapProperties();
            Bounds bounds1 = new Bounds(new Vector3(2, 0, -2), new Vector3(0.5f, 1f, 0.5f));
            Bounds bounds2 = new Bounds(new Vector3(5, 0, -5), new Vector3(0.5f, 1f, 0.5f));
            Vector3[] corners1 = Pathfinding.PathfindingObstacle.CalculateCorners(bounds1.center, bounds1.extents.x + 0.5f, Vector3.right, bounds1.extents.z + 0.5f, Vector3.forward);
            Vector3[] corners2 = Pathfinding.PathfindingObstacle.CalculateCorners(bounds2.center, bounds2.extents.x + 0.5f, Vector3.right, bounds2.extents.z + 0.5f, Vector3.forward);
            Pathfinding.PathfindingGraphGenerator.AddObstacleData(bounds1, corners1);
            Pathfinding.PathfindingGraphGenerator.AddObstacleData(bounds2, corners2);
            Assert.AreEqual(corners1.Length + corners2.Length + 1, Pathfinding.PathfindingGraphGenerator.GetPathfindingGraph().Length);
        }

        [TearDown]
        public void ResetScene()
        {
            TestReseter.TearDownScene();
        }
    }
}
