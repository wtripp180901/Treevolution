using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

public class BuddyTests
{
    [UnityTest]
    public IEnumerator TestNewInstructionsCleanCommands()
    {
        GameObject closerEnemy, furtherEnemy;
        DictationTests.setupAttackScene(out closerEnemy, out furtherEnemy);
        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "the", "ant." }, (x => results = x));
        BuddyScript buddyScript = GameObject.FindWithTag("Buddy").GetComponent<BuddyScript>();
        buddyScript.GiveInstructions(results);
        yield return null;
        GameObject target;
        Queue<GameObject> targets;
        bool isok;
        buddyScript.GetTestData(out targets, out target, out isok);
        Assert.AreSame(furtherEnemy, target);
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "the", "hornet." }, (x => results = x));
        buddyScript.GiveInstructions(results);
        yield return null;
        buddyScript.GetTestData(out targets, out target, out isok);
        Assert.AreSame(target, closerEnemy);

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
