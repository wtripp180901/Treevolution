using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Tests;

public class DictationTests
{
    [UnitySetUp]
    public IEnumerator Init()
    {
        // in most play mode test cases you would want to at least create an MRTK GameObject using the default profile
        TestUtilities.InitializeMixedRealityToolkit(true);
        yield return null;
    }

    // Destroy the scene - this method is called after each test listed below has completed
    // Note that this uses UnityTearDown instead of [TearDown] because the init function needs to await a frame passing
    // to ensure that the MRTK system has fully torn down before the next test setup->run cycle starts.
    [UnityTearDown]
    public IEnumerator TearDown()
    {
        PlayModeTestUtilities.TearDown();
        yield return null;
    }

    [UnityTest]
    public IEnumerator testBasicMovementCommand()
    {
        GameObject logic = new GameObject();
        GameObject pointer = new GameObject();
        pointer.transform.position = new Vector3(0, 0, 0);
        PointerLocationTracker pointerTracker = logic.AddComponent<PointerLocationTracker>();
        pointerTracker.pointer = pointer;
        List<BuddyAction> result = null;
        yield return new LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(new string[]{ "go", "here."}, (x => result = x));
        Assert.AreEqual(result.Count, 1);
    }

}
