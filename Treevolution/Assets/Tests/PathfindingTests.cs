using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PathfindingTests
{
    // A Test behaves as an ordinary method
    [UnityTest]
    public IEnumerator PathfinderWorksWithoutGraph()
    {
        /*GameObject tree = new GameObject();
        tree.tag = "Tree";
        //tree.AddComponent<Transform>();
        tree.transform.position = new Vector3(0, 0, 0);
        //Assert.AreEqual(Pathfinding.PathfindingGraphGenerator.GetPathfindingGraph().Length, 0,"error");
        Assert.AreEqual(Pathfinding.Pathfinder.GetPath(new Vector3(0, 0, 0), new Vector3(0,0,0)).Length, 1);*/
        yield return null;
    }

    [Test]
    public void PathfindingGraphGeneratorWorksWithoutObstacles()
    {
        //Assert.AreEqual(Pathfinding.PathfindingGraphGenerator.GetPathfindingGraph().Length, 0);
    }
}
