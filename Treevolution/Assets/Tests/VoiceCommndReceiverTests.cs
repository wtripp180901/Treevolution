using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VoiceCommandReceiverTests
{
    [Test]
    public void TestBasicBuddyCommandsPropagateToBuddy()
    {
        GameObject enemy1, enemy2, pointer;
        DictationTests.setupAttackScene(out pointer, out enemy1, out enemy2);
        GameObject logic = GameObject.FindWithTag("Logic");
        logic.AddComponent<VoiceCommandReceiver>();
        VoiceCommandReceiver vcr = logic.GetComponent<VoiceCommandReceiver>();
        vcr.SetupForTest();
        vcr.pointer = pointer;
        vcr.BuddyAttack();
        BuddyScript buddyScript = GameObject.FindWithTag("Buddy").GetComponent<BuddyScript>();
        Queue<BuddyAction> actions;
        buddyScript.GetTestData(out actions);
        Assert.AreEqual(1,actions.Count);
        BuddyAction currentAction = actions.Dequeue();
        Assert.AreEqual(BUDDY_ACTION_TYPES.Attack, currentAction.actionType);

        vcr.BuddyRepair();
        buddyScript.GetTestData(out actions);
        currentAction = actions.Dequeue();
        Assert.AreEqual(BUDDY_ACTION_TYPES.Repair, currentAction.actionType);

        vcr.BuddyDefend();
        buddyScript.GetTestData(out actions);
        Assert.AreEqual(2, actions.Count);
        currentAction = actions.Dequeue();
        Assert.AreEqual(BUDDY_ACTION_TYPES.Move, currentAction.actionType);
        currentAction = actions.Dequeue();
        Assert.AreEqual(BUDDY_ACTION_TYPES.Defend, currentAction.actionType);
    }
    [UnityTest]
    public IEnumerator TestLightningStrike()
    {
        Rigidbody dummy;
        GameObject enemy;
        EnemyScript enemyScript;
        GameObject logic;
        EnemyTests.CreateSceneWithEnemies(out dummy, out enemy, out enemyScript, out logic);
        logic.GetComponent<EnemyManager>().AddToSceneAsEnemyForTest(enemy);
        GameObject pointer = new GameObject();
        logic.AddComponent<AudioSource>();
        logic.AddComponent<VoiceCommandReceiver>();
        VoiceCommandReceiver vcr = logic.GetComponent<VoiceCommandReceiver>();
        vcr.SetupForTest();
        vcr.pointer = pointer;
        pointer.AddComponent<MeshRenderer>();

        int prevHealth = enemyScript.health;

        vcr.LightningBolt();

        Assert.Less(enemyScript.health, prevHealth);

        Assert.AreEqual(pointer.GetComponent<Renderer>().material.color,Color.red);
        yield return new WaitForSeconds(0.4f);
        Assert.AreNotEqual(pointer.GetComponent<Renderer>().material.color, Color.red);

    }

    [TearDown]
    public void ResetScene()
    {
        TestReseter.TearDownScene();
    }
}
