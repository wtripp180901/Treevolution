using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyManagerTests
{
    [UnityTest]
    public IEnumerator SuccessfullySpawnsFirstEnemy()
    {

        GameObject logic = new GameObject();
        GameObject ant;
        Rigidbody dummy;
        EnemyScript dummy1;
        EnemyTests.CreateSceneWithEnemies(out dummy, out ant, out dummy1, out logic);

        logic.AddComponent<UIController>();
        logic.GetComponent<UIController>().SetupForTest();
        logic.AddComponent<RoundTimer>();
        GameProperties.SetTestProperties(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.one, Vector3.zero, Pose.identity, 0);
        EnemyManager enemyManager = logic.GetComponent<EnemyManager>();
        enemyManager.antPrefab = ant;

        yield return new WaitForEndOfFrame();
        logic.GetComponent<RoundTimer>().StartTimer();

        enemyManager.StartSpawning(new Dictionary<GameStateManager.EnemyType, int>() { { GameStateManager.EnemyType.Ant, 10 } });
        
        yield return new WaitForEndOfFrame();
        Assert.AreEqual(1, enemyManager.enemies.Length);

    }

    [TearDown]
    public void ResetScene()
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