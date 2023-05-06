using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestReseter
{
    public static void TearDownScene()
    {
        Object[] all = GameObject.FindObjectsOfType(typeof(GameObject));
        for (int i = 0; i < all.Length; i++)
        {
            GameObject dirty = (GameObject)(all[i]);
            Pathfinding.PathfindingObstacle dirtyObstacleScript = dirty.GetComponent<Pathfinding.PathfindingObstacle>();
            if (dirtyObstacleScript != null) dirtyObstacleScript.CleanForTest();
            Object.Destroy(all[i]);
        }
    }
}
