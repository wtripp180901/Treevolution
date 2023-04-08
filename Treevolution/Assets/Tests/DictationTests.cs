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

}
