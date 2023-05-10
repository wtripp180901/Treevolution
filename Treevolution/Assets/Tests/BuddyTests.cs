using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using BuddyActions;

namespace Tests
{
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

        [UnityTest]
        public IEnumerator TestDefendFindsNearest()
        {
            GameObject closerEnemy, furtherEnemy;
            DictationTests.setupAttackScene(out closerEnemy, out furtherEnemy);
            List<BuddyAction> results = null;
            yield return new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[] { "defend." }, (x => results = x));
            BuddyScript buddyScript = GameObject.FindWithTag("Buddy").GetComponent<BuddyScript>();
            buddyScript.GiveInstructions(results);
            yield return null;
            Queue<GameObject> targets;
            GameObject currentTarget;
            bool dummy;
            buddyScript.GetTestData(out targets, out currentTarget, out dummy);
            Assert.AreEqual(closerEnemy, currentTarget);
        }

        [UnityTest]
        public IEnumerator TestTargettedActionWorksWithSingleTargetInit()
        {
            GameObject closerEnemy, furtherEnemy;
            DictationTests.setupAttackScene(out closerEnemy, out furtherEnemy);
            BuddyScript buddyScript = GameObject.FindWithTag("Buddy").GetComponent<BuddyScript>();
            buddyScript.GiveInstructions(new List<BuddyAction>() { new TargetedBuddyAction(BUDDY_ACTION_TYPES.Attack, closerEnemy) });
            yield return null;
            Queue<GameObject> targets;
            GameObject currentTarget;
            bool isok;
            buddyScript.GetTestData(out targets, out currentTarget,out isok);
            Assert.IsEmpty(targets);
            Assert.AreEqual(currentTarget, closerEnemy);
            Assert.IsFalse(isok);
        }

        [UnityTest]
        public IEnumerator TestBuddyMoveAction()
        {
            GameObject buddy = new GameObject();
            buddy.transform.position = new Vector3(0, 0, 0);
            buddy.AddComponent<Rigidbody>();
            buddy.AddComponent<BuddyScript>();

            BuddyScript script = buddy.GetComponent<BuddyScript>();
            buddy.GetComponent<Rigidbody>().useGravity = false;
            script.SetupForTest(1f);

            yield return null;

            script.GiveInstructions(new List<BuddyAction>() { new MoveBuddyAction(new Vector3(1, 0, 0)) });

            yield return null;

            yield return new WaitForFixedUpdate();
            Assert.Greater(buddy.GetComponent<Rigidbody>().transform.position.magnitude, 0);
            yield return new WaitForSeconds(1f);
            Assert.Less((buddy.GetComponent<Rigidbody>().position - new Vector3(1, 0, 0)).magnitude, 0.05f);

        }

        [TearDown]
        public void ResetScene()
        {
            TestReseter.TearDownScene();
        }
    }
}
