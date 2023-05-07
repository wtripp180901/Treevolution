using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

namespace Tests
{
    public class EnemyTests
    {

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NormalEnemiesDamagedByWalls()
        {
            Rigidbody rig;
            EnemyScript enemyScript;
            GameObject logic;
            GameObject enemy;
            CreateSceneWithEnemies(out rig, out enemy, out enemyScript, out logic);
            rig.MovePosition(new Vector3(5, 0, 0));

            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //wall.AddComponent<BoxCollider>();
            wall.GetComponent<BoxCollider>().isTrigger = true;

            bool inWall;
            int health;
            enemyScript.GetTestData(out inWall, out health);

            Assert.IsFalse(inWall);
            yield return null;

            rig.MovePosition(wall.transform.position);

            yield return new WaitForFixedUpdate();
            enemyScript.GetTestData(out inWall, out health);

            int prevHealth = health;
            Assert.IsTrue(inWall);
            yield return new WaitForSeconds(1);
            enemyScript.GetTestData(out inWall, out health);
            Assert.AreEqual(prevHealth > health, true);

        }

        public static void CreateSceneWithEnemies(out Rigidbody enemyRb, out GameObject enemy, out EnemyScript enemyScript, out GameObject logic)
        {
            logic = new GameObject();
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

            createBasicEnemy(out enemy, out enemyRb, out enemyScript);
        }

        public static void createBasicEnemy(out GameObject enemy, out Rigidbody enemyRb, out EnemyScript enemyScript)
        {
            enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy.AddComponent<AudioSource>();
            enemy.AddComponent<HealthBar>();
            enemy.AddComponent<UnityEngine.UI.Slider>();
            enemy.GetComponent<HealthBar>().SetupForTest(enemy.GetComponent<UnityEngine.UI.Slider>(), new TextMeshProUGUI());
            enemy.AddComponent<EnemyScript>();
            enemy.AddComponent<BoxCollider>();
            enemy.AddComponent<Rigidbody>();
            enemy.tag = "Enemy";
            enemyRb = enemy.GetComponent<Rigidbody>();
            enemyRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            enemyRb.useGravity = false;
            enemyScript = enemy.GetComponent<EnemyScript>();
            enemyScript.geometry = enemy;
        }

        [TearDown]
        public void ResetScene()
        {
            TestReseter.TearDownScene();
        }
    }
}
