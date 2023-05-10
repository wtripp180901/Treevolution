using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Towers;

namespace Tests
{
    public class TowerTests
    {
        [UnityTest]
        public IEnumerator TestSingleTargetTower()
        {
            Rigidbody enemyRb;
            GameObject enemy;
            EnemyScript enemyScript;
            GameObject logic;
            EnemyTests.CreateSceneWithEnemies(out enemyRb, out enemy, out enemyScript, out logic);
            logic.GetComponent<EnemyManager>().AddToSceneAsEnemyForTest(enemy);

            enemy.name = "enemy";
            enemy.tag = "Enemy";

            logic.AddComponent<TowerManager>();

            enemy.transform.position = new Vector3(0.005f, 0, 0);

            GameObject singleTargetTower = new GameObject();
            singleTargetTower.tag = "Tower";
            singleTargetTower.AddComponent<Gun>();
            singleTargetTower.AddComponent<SingleTargetTower>();
            singleTargetTower.GetComponent<SingleTargetTower>().SetupForTest(0.3f, singleTargetTower);

            singleTargetTower.transform.position = new Vector3(0, 0, 0);

            GameObject gunPoint = new GameObject();
            gunPoint.transform.position = singleTargetTower.transform.position;

            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "Bullet";
            projectile.transform.position = new Vector3(-10, 0, 0);
            projectile.tag = "Bullet";
            projectile.AddComponent<BulletScript>();
            projectile.AddComponent<Rigidbody>();
            projectile.GetComponent<Rigidbody>().useGravity = false;

            singleTargetTower.GetComponent<Gun>().SetupForTest(projectile, new GameObject[] { gunPoint });

            int prevEnemyHealth;
            bool dummy;
            enemyScript.GetTestData(out dummy, out prevEnemyHealth);

            yield return null;
            enemyRb.useGravity = false;

            yield return new WaitForSeconds(1);

            int currentHealth;
            enemyScript.GetTestData(out dummy, out currentHealth);

            Assert.AreEqual(prevEnemyHealth > currentHealth, true);

        }

        [UnityTest]
        public IEnumerator TestDigestTower()
        {
            Rigidbody enemyRb;
            GameObject enemy;
            EnemyScript enemyScript;
            GameObject logic;
            EnemyTests.CreateSceneWithEnemies(out enemyRb, out enemy, out enemyScript, out logic);
            enemy.transform.position = new Vector3(1, 0, 0);
            logic.GetComponent<EnemyManager>().AddToSceneAsEnemyForTest(enemy);
            logic.AddComponent<TowerManager>();
            GameObject digestTower = new GameObject();
            digestTower.transform.position = new Vector3(0, 0, 0);
            digestTower.tag = "Tower";
            digestTower.AddComponent<DigestTower>();
            DigestTower towerScript = digestTower.GetComponent<DigestTower>();
            towerScript.SetupForTest(1);
            digestTower.AddComponent<Animator>();


            enemyRb.MovePosition(new Vector3(0, 0, 0));

            bool digesting;
            towerScript.GetTestData(out digesting);

            Assert.IsFalse(digesting);
            int prevHealth = enemyScript.health;

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(0.05f);


            towerScript.GetTestData(out digesting);
            Assert.IsTrue(digesting);

            yield return new WaitForSeconds(0.5f);
            Assert.Less(enemyScript.health, prevHealth);

        }

        [UnityTest]
        public IEnumerator TestMultiTargetTower()
        {
            Rigidbody enemyRb;
            GameObject enemy;
            EnemyScript enemyScript;
            GameObject logic;
            EnemyTests.CreateSceneWithEnemies(out enemyRb, out enemy, out enemyScript, out logic);
            enemyRb.useGravity = false;
            enemy.transform.position = new Vector3(0.5f, 0, 0);
            logic.GetComponent<EnemyManager>().AddToSceneAsEnemyForTest(enemy);

            GameObject enemy1;
            EnemyScript enemyScript1;
            EnemyTests.createBasicEnemy(out enemy1, out enemyRb, out enemyScript1);
            enemyRb.useGravity = false;
            enemy1.transform.position = new Vector3(-0.5f, 0, 0);
            logic.GetComponent<EnemyManager>().AddToSceneAsEnemyForTest(enemy1);

            logic.AddComponent<TowerManager>();
            GameObject multiTargetTower = new GameObject();
            multiTargetTower.transform.position = new Vector3(0, 0, 0);
            multiTargetTower.tag = "Tower";
            multiTargetTower.AddComponent<MultiTargetTower>();
            MultiTargetTower towerScript = multiTargetTower.GetComponent<MultiTargetTower>();
            towerScript.SetupForTest(1);

            int prevHealth = enemyScript.health;
            int prevHealth1 = enemyScript1.health;

            yield return new WaitForSeconds(1f);

            Assert.IsTrue(prevHealth > enemyScript.health && prevHealth1 > enemyScript1.health);
        }

        [TearDown]
        public void ResetScene()
        {
            TestReseter.TearDownScene();
        }
    }
}
