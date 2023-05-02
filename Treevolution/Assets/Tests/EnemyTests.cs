using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyTests
{

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NormalEnemiesDamagedByWalls()
    {
        GameObject logic = new GameObject();
        logic.AddComponent<EnemyManager>();
        logic.AddComponent<RoundTimer>();
        logic.GetComponent<EnemyManager>().SetupForTest();
        logic.tag = "Logic";
        logic.name = "Logic";

        GameObject rInd = new GameObject();
        GameObject lInd = new GameObject();
        rInd.AddComponent<MeshRenderer>();
        lInd.AddComponent<MeshRenderer>();
        rInd.tag = "RightIndicator";
        lInd.tag = "LeftIndicator";
        lInd.AddComponent<SpawnDirectionIndicator>();
        rInd.AddComponent<SpawnDirectionIndicator>();

        GameObject tree = new GameObject();
        tree.tag = "Tree";

        GameObject cam = new GameObject("MainCamera");
        cam.tag = "MainCamera";
        cam.AddComponent<Camera>();
        
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //wall.AddComponent<BoxCollider>();
        wall.GetComponent<BoxCollider>().isTrigger = true;

        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemy.AddComponent<AudioSource>();
        enemy.AddComponent<HealthBar>();
        enemy.AddComponent<UnityEngine.UI.Slider>();
        enemy.GetComponent<HealthBar>().slider = enemy.GetComponent<UnityEngine.UI.Slider>();
        enemy.AddComponent<EnemyScript>();
        enemy.AddComponent<BoxCollider>();
        enemy.transform.position = new Vector3(5, 0, 0);
        enemy.AddComponent<Rigidbody>();
        Rigidbody rig = enemy.GetComponent<Rigidbody>();
        rig.collisionDetectionMode = CollisionDetectionMode.Continuous;
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();

        bool inWall;
        int health;
        enemyScript.GetTestData(out inWall,out health);

        Assert.IsFalse(inWall);
        yield return null;

        rig.MovePosition(wall.transform.position);

        yield return new WaitForFixedUpdate();
        enemyScript.GetTestData(out inWall,out health);

        int prevHealth = health;
        Assert.IsTrue(inWall);
        yield return new WaitForSeconds(1);
        enemyScript.GetTestData(out inWall, out health);
        Assert.AreEqual(prevHealth > health, true);
        
    }
}
