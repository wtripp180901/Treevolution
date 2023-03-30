using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

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
        Assert.AreEqual(Pathfinding.Pathfinder.GetPath(new Vector3(0, 0, 0), new Vector3(0, 0, 0)).Length, 1);
        yield return null;
    }

    [Test]
    public void PathfindingGraphGeneratorWorksWithoutObstacles()
    {
        Assert.AreEqual(Pathfinding.PathfindingGraphGenerator.GetPathfindingGraph().Length, 0);
    }

    [Test]
    public void PathfindingGraphGeneratorWorksWithObstacles()
    {
        Bounds bounds1 = new Bounds(new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 0.5f));
        Bounds bounds2 = new Bounds(new Vector3(-2f, -2f, -2f), new Vector3(0.5f, 0.5f, 0.5f));
        Pathfinding.PathfindingGraphGenerator.AddObstacleData(bounds1, new Vector3[] { Vector3.one, Vector3.forward, Vector3.right });
        Pathfinding.PathfindingGraphGenerator.AddObstacleData(bounds2, new Vector3[] { -Vector3.one, -Vector3.forward, -Vector3.right });
        Assert.AreEqual(Pathfinding.PathfindingGraphGenerator.GetPathfindingGraph().Length, 6);
    }
}
