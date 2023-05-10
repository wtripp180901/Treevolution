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

        [UnityTest]
        public IEnumerator PathfindingGraphGeneratorWorksWithoutObstacles()
        {
            GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tree.tag = "Tree";
            GameObject.Instantiate(tree);
            Assert.AreEqual(1, Pathfinding.PathfindingGraphGenerator.GetPathfindingGraph().Length);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PathfindingGraphGeneratorWorksWithObstacles()
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
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestBattleMovementPenaltyAppliedForWalls()
        {
            GameProperties.BattlePhase = true;
            GameObject wall = new GameObject();
            WallScript wallScript = wall.AddComponent<WallScript>();
            wallScript.SetupForTest(wall.AddComponent<BoxCollider>(), wall.AddComponent<Pathfinding.PathfindingObstacle>());
            RuntimeMovable runtimeMovable = wall.AddComponent<RuntimeMovable>();
            runtimeMovable.SetupForTest(wallScript, 0.1f);
            yield return null;
            wall.transform.position = new Vector3(5, 0, 0);
            yield return new WaitForSeconds(0.05f);
            Assert.IsFalse(wallScript.GetComponent<Collider>().enabled);
            yield return new WaitForSeconds(0.06f);
            Assert.IsTrue(wallScript.GetComponent<Collider>().enabled);

        }

        [UnityTest]
        public IEnumerator PathfinderNavigatesAroundNonTrivialPaths()
        {
            GameObject tree = new GameObject();
            tree.tag = "Tree";
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameProperties.SetTestProperties(new Vector3(-10, 0, 0), new Vector3(10, 0, 0), new Vector3(-10, 0, -10), new Vector3(10, 0, -10), new Vector3(10, 0, 10), new Vector3(0, 0, 0), Pose.identity, 0);
            obstacle.AddComponent<Pathfinding.PathfindingObstacle>();
            obstacle.transform.position = new Vector3(3, 0, 0);
            obstacle.layer = 3;
            yield return null;

            Vector3[] path = Pathfinding.Pathfinder.GetPath(new Vector3(0, 0, 0), new Vector3(5, 0, 0), true);
            yield return new WaitForFixedUpdate();
            Assert.Greater(path.Length, 2);
            Assert.Less((path[path.Length - 1] - new Vector3(5, 0, 0)).magnitude, 0.01);
        }

        [UnityTest]
        public IEnumerator TestBattleMovementPenaltyAppliedForTowers()
        {
            GameObject logic = new GameObject();
            logic.name = "Logic";
            logic.tag = "Logic";

            logic.AddComponent<Towers.TowerManager>();
            logic.AddComponent<EnemyManager>();

            GameObject rangeVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            
            GameProperties.BattlePhase = true;
            GameObject disabledVisualObject = new GameObject();
            disabledVisualObject.AddComponent<MeshRenderer>();
            GameObject tower = new GameObject();
            RuntimeMovable runtimeMovable = tower.AddComponent<RuntimeMovable>();
            Towers.SingleTargetTower towerScript = tower.AddComponent<Towers.SingleTargetTower>();
            towerScript.rangeVisual = rangeVisual;
            towerScript.disabledVisualObject = disabledVisualObject;
            runtimeMovable.SetupForTest(towerScript, 0.1f);
            yield return null;
            tower.transform.position = new Vector3(5, 0, 0);
            yield return new WaitForSeconds(0.05f);
            bool shootingEnabled = false;
            towerScript.GetTestData(out shootingEnabled);
            Assert.IsFalse(shootingEnabled);
            yield return new WaitForSeconds(0.06f);
            towerScript.GetTestData(out shootingEnabled);
            Assert.IsTrue(shootingEnabled);

        }

        [TearDown]
        public void ResetScene()
        {
            TestReseter.TearDownScene();
        }
    }
}
