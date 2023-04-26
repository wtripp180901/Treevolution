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
        Debug.Log(results);
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

    [UnityTest]
    public IEnumerator NoSubjectAttackDefaults()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 2);
        Assert.Contains(closerEnemy, ((TargetedBuddyAction)results[0]).targets);
        Assert.Contains(furtherEnemy, ((TargetedBuddyAction)results[0]).targets);
    }

    [UnityTest]
    public IEnumerator BuddyTargetsClosestOfMultipleFirst()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 2);
        Assert.AreSame(closerEnemy, ((TargetedBuddyAction)results[0]).targets[0]);
        Assert.AreSame(furtherEnemy, ((TargetedBuddyAction)results[0]).targets[1]);
    }

    [UnityTest]
    public IEnumerator AttackWithPluralNounAndNoSubject()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "ants." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.Contains(furtherEnemy, ((TargetedBuddyAction)results[0]).targets);
    }

    [UnityTest]
    public IEnumerator SelectsCorrectSingularTargetForRestrictions()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "that", "ant." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.AreSame(((TargetedBuddyAction)results[0]).targets[0], furtherEnemy);
    }

    [UnityTest]
    public IEnumerator SelectsBasedOnAdjectives()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);
        GameObject otherEnemy = new GameObject();
        GameObject.FindWithTag("Logic").GetComponent<EnemyManager>().AddToSceneAsEnemyForTest(otherEnemy);
        otherEnemy.AddComponent<BuddyInteractable>();
        otherEnemy.GetComponent<BuddyInteractable>().SetupForTest(new RESTRICTION_TYPES[] { RESTRICTION_TYPES.ArmouredBug, RESTRICTION_TYPES.Small, RESTRICTION_TYPES.Grounded, RESTRICTION_TYPES.Armoured });

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "the", "little", "bugs." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 2);
        Assert.Contains(furtherEnemy,((TargetedBuddyAction)results[0]).targets);
        Assert.Contains(otherEnemy, ((TargetedBuddyAction)results[0]).targets);
    }

    [UnityTest]
    public IEnumerator AppliesRestrictionsBasedOnAdditionalNouns()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "the", "ones", "in", "the", "air" }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.Contains(closerEnemy, ((TargetedBuddyAction)results[0]).targets);

        PointerLocationTracker tracker = GameObject.FindWithTag("Logic").GetComponent<PointerLocationTracker>();
        tracker.StartSampling();
        tracker.FinishSampling();

        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "the", "bugs", "on", "the", "ground" }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.Contains(furtherEnemy, ((TargetedBuddyAction)results[0]).targets);

        tracker.StartSampling();
        tracker.FinishSampling();

        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "the", "bugs", "with", "armor" }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.Contains(closerEnemy, ((TargetedBuddyAction)results[0]).targets);

    }

    [UnityTest]
    public IEnumerator DifferentRestrictionsAreAppliedToDifferentSubjects()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);

        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "that", "ant", "and", "then", "attack", "that", "hornet." }, (x => results = x));
        Assert.AreEqual(results.Count, 2);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 1);
        Assert.AreSame(((TargetedBuddyAction)results[0]).targets[0], furtherEnemy);
        Assert.AreEqual(((TargetedBuddyAction)results[1]).targets.Length, 1);
        Assert.AreSame(((TargetedBuddyAction)results[1]).targets[0], closerEnemy);
    }

    [UnityTest]
    public IEnumerator TestPreviouslyFailedCommands()
    {
        GameObject closerEnemy, furtherEnemy;
        setupAttackScene(out closerEnemy, out furtherEnemy);
        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "attack", "those", "ones", "over", "there." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 2);
        Assert.Contains(furtherEnemy, ((TargetedBuddyAction)results[0]).targets);
        Assert.Contains(closerEnemy, ((TargetedBuddyAction)results[0]).targets);
    }

    [UnityTest]
    public IEnumerator RepairsDamagedOrBrokenWallsByDictation()
    {
        GameObject buddy = new GameObject();
        buddy.tag = "Buddy";
        GameObject normalWall = new GameObject();
        GameObject damagedWall = new GameObject();
        GameObject brokenWall = new GameObject();
        WallScript currentWall;
        WallBuddyInteractable dummy;
        initialiseWall(normalWall,out dummy,out currentWall);
        initialiseWall(damagedWall, out dummy, out currentWall);
        currentWall.Damage(5);
        initialiseWall(brokenWall, out dummy, out currentWall);
        currentWall.Damage(10);
        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "repair", "the", "walls." }, (x => results = x));
        Assert.AreEqual(results.Count, 1);
        Assert.AreEqual(((TargetedBuddyAction)results[0]).targets.Length, 2);
        Assert.Contains(damagedWall, ((TargetedBuddyAction)results[0]).targets);
        Assert.Contains(brokenWall, ((TargetedBuddyAction)results[0]).targets);
    }

    [UnityTest]
    public IEnumerator DefendDefendsWalls()
    {
        GameObject closerEnemy, furtherEnemy;
        GameObject wall = new GameObject();
        WallBuddyInteractable dummy;
        WallScript wDummy;
        initialiseWall(wall, out dummy,out wDummy);
        DictationTests.setupAttackScene(out closerEnemy, out furtherEnemy);
        List<BuddyAction> results = null;
        yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "defend","the","wall." }, (x => results = x));
        Assert.AreEqual(results.Count, 2);
        Assert.AreEqual(results[0].actionType, BUDDY_ACTION_TYPES.Move);
        Assert.AreEqual(results[1].actionType, BUDDY_ACTION_TYPES.Defend);
        Assert.LessOrEqual((((MoveBuddyAction)results[0]).location - wall.transform.position).magnitude,0.01f);
    }

    void initialiseWall(GameObject wall, out WallBuddyInteractable buddyInteractable, out WallScript wallScript)
    {
        buddyInteractable = wall.AddComponent<WallBuddyInteractable>();
        wallScript = wall.AddComponent<WallScript>();
        wall.AddComponent<BoxCollider>();
        wall.AddComponent<Pathfinding.PathfindingObstacle>();
        wall.tag = "Wall";
        wallScript.SetupForTest(wall.GetComponent<Collider>(), wall.GetComponent<Pathfinding.PathfindingObstacle>());
        buddyInteractable.SetupForTest(new RESTRICTION_TYPES[] { RESTRICTION_TYPES.Wall }, wallScript);

    }

    static void setupBasicMovementScene(out GameObject pointer)
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

    public static void setupAttackScene(out GameObject closerEnemy,out GameObject furtherEnemy)
    {
        GameObject pointer;
        setupAttackScene(out pointer,out closerEnemy,out furtherEnemy);
    }

    public static void setupAttackScene(out GameObject pointer,out GameObject closerEnemy, out GameObject furtherEnemy)
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
        closerEnemy.GetComponent<BuddyInteractable>().SetupForTest(new RESTRICTION_TYPES[] { RESTRICTION_TYPES.Hornet, RESTRICTION_TYPES.Flying, RESTRICTION_TYPES.Armoured });
        furtherEnemy.GetComponent<BuddyInteractable>().SetupForTest(new RESTRICTION_TYPES[] { RESTRICTION_TYPES.Ant, RESTRICTION_TYPES.Small, RESTRICTION_TYPES.Grounded, RESTRICTION_TYPES.Unarmoured });
        GameObject buddy = new GameObject();
        buddy.tag = "Buddy";
        buddy.AddComponent<Rigidbody>();
        buddy.AddComponent<BuddyScript>();
        buddy.GetComponent<BuddyScript>().SetupForTest();
        buddy.transform.position = new Vector3(0, 0, 0);
    }

    [TearDown]
    public void ResetScene()
    {
        Object[] all = GameObject.FindObjectsOfType(typeof(GameObject));
        for(int i =0;i < all.Length; i++)
        {
            GameObject dirty = (GameObject)(all[i]);
            Pathfinding.PathfindingObstacle dirtyObstacleScript = dirty.GetComponent<Pathfinding.PathfindingObstacle>();
            if (dirtyObstacleScript != null) dirtyObstacleScript.CleanForTest();
            Object.Destroy(all[i]);
        }
    }

}
