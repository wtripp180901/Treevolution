using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

namespace Tests
{
    public class MiscTests
    {

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestDebuggerReportsBugsWhenDeployed()
        {
            GameObject logic = new GameObject();
            logic.AddComponent<RoundTimer>();
            logic.AddComponent<EnemyManager>().SetupForTest();
            logic.AddComponent<GameStateManager>();
            logic.AddComponent<Debugger>();
            logic.AddComponent<UIController>();
            UIController uiController = logic.GetComponent<UIController>();
            uiController.SetupForTest();
            logic.AddComponent<TextMeshProUGUI>();
            uiController.dictationText = logic.GetComponent<TextMeshProUGUI>();

            yield return null;

            Debug.LogError("Intentional error triggered");
            LogAssert.Expect(LogType.Error, "Intentional error triggered");

            yield return null;
            Assert.AreNotEqual("", uiController.dictationText.text);

        }

        [TearDown]
        public void ResetScene()
        {
            TestReseter.TearDownScene();
        }
    }
}
