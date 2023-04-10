using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

public class DictationTests
{

    [UnityTest]
    public IEnumerator testBasicMovementCommand()
    {
        GameObject pointer;
        setupBasicMovementScene(out pointer);

        List<BuddyAction> result = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[]{ "go", "here."}, (x => result = x));
        Assert.AreEqual(((MoveBuddyAction)result[0]).location, pointer.transform.position);
    }

    [UnityTest]
    public IEnumerator BasicMovementWorksWithWordNotInBasewords()
    {
        GameObject pointer;
        setupBasicMovementScene(out pointer);

        List<BuddyAction> result = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "proceed", "here." }, (x => result = x));
        Assert.AreEqual(((MoveBuddyAction)result[0]).location, pointer.transform.position);
    }

    [UnityTest]
    public IEnumerator SingularAttackWorksWithObjectivePronoun()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack","that" ,"one." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.AreSame(((TargetedBuddyAction)results[0]).targets[0], closerEnemy);
    }

    [UnityTest]
    public IEnumerator SingularAttackWorksWithSubjectivePronoun()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "it." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.AreSame(((TargetedBuddyAction)results[0]).targets[0], closerEnemy);
    }

    [UnityTest]
    public IEnumerator SingularAttackAtPronounSubjectGetsClosest()
    {
        GameObject closerEnemy, furtherEnemy, pointer;
        setupAttackScene(out pointer, out closerEnemy, out furtherEnemy);

        PointerLocationTracker tracker = GameObject.FindWithTag("Logic").GetComponent<PointerLocationTracker>();

        List<BuddyAction> results = null;
        tracker.FinishSampling();
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "it" }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.AreSame(((TargetedBuddyAction)results[0]).targets[0], closerEnemy);
        pointer.transform.position = new Vector3(0.15f, 0, 0);
        tracker.StartSampling();
        tracker.FinishSampling();
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "it" }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.AreSame(((TargetedBuddyAction)results[0]).targets[0], furtherEnemy);
    }

    [UnityTest]
    public IEnumerator MultiAttackWorksWithObjectivePronoun()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "those", "ones." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 2);
        Assert.Contains(closerEnemy, ((TargetedBuddyAction)results[0]).targets);
        Assert.Contains(furtherEnemy, ((TargetedBuddyAction)results[0]).targets);
    }

    [UnityTest]
    public IEnumerator MultiAttackWorksWithSubjectivePronoun()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "them." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 2);
        Assert.Contains(closerEnemy, ((TargetedBuddyAction)results[0]).targets);
        Assert.Contains(furtherEnemy, ((TargetedBuddyAction)results[0]).targets);
    }

    void setupBasicMovementScene(out GameObject pointer)
    {
        GameObject logic = new GameObject();
        logic.tag = "Logic";

        GameObject moveToMarker = new GameObject();
        moveToMarker.tag = "MoveToMarker";

        pointer = new GameObject();
        pointer.transform.position = new Vector3(0, 0, 0);
        PointerLocationTracker pointerTracker = logic.AddComponent<PointerLocationTracker>();
        pointerTracker.pointer = pointer;
        pointerTracker.StartSampling();
        pointerTracker.FinishSampling();
    }

    void setupAttackScene(out GameObject closerEnemy,out GameObject furtherEnemy)
    {
        GameObject pointer;
        setupAttackScene(out pointer,out closerEnemy,out furtherEnemy);
    }

    void setupAttackScene(out GameObject pointer,out GameObject closerEnemy, out GameObject furtherEnemy)
    {
        setupBasicMovementScene(out pointer);
        GameObject logic = GameObject.FindWithTag("Logic");
        EnemyManager manager = logic.AddComponent<EnemyManager>();
        closerEnemy = new GameObject();
        closerEnemy.transform.position = new Vector3(0.1f, 0, 0);
        closerEnemy.AddComponent<BuddyInteractable>();
        furtherEnemy = new GameObject();
        furtherEnemy.transform.position = new Vector3(0.15f, 0, 0);
        furtherEnemy.AddComponent<BuddyInteractable>();
        manager.AddToSceneAsEnemyForTest(closerEnemy);
        manager.AddToSceneAsEnemyForTest(furtherEnemy);
    }

    [TearDown]
    public void ResetScene()
    {
        Object[] all = GameObject.FindObjectsOfType(typeof(GameObject));
        for(int i =0;i < all.Length; i++)
        {
            Object.Destroy(all[i]);
        }
    }

}
