using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameStateManagerTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void GameStateManagerStateTransitions()
    {
        // Calibration
        GameObject logic = new GameObject();
        GameStateManager gsm = logic.AddComponent<GameStateManager>();
        gsm.SetupGameStateManagerTesting();
        GameProperties.SetTestProperties(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.one, Vector3.zero, Pose.identity, 0);
        Assert.AreEqual(GameStateManager.GameState.Calibration, gsm.currentGameState);

        // Calibration Success
        gsm.CalibrationSuccess();
        Assert.AreEqual(GameStateManager.GameState.Calibration_Success, gsm.currentGameState);

        // Tutorial Plan
        gsm.BeginTutorialPlan();
        Assert.AreEqual(GameStateManager.GameState.Tutorial_Plan, gsm.currentGameState);

        // Tutorial Battle
        gsm.BeginBattle();
        Assert.AreEqual(GameStateManager.GameState.Tutorial_Battle, gsm.currentGameState);

        // Round Plan
        gsm.BeginRound();
        Assert.AreEqual(GameStateManager.GameState.Round_Plan, gsm.currentGameState);

        // Round Battle
        gsm.BeginBattle();
        Assert.AreEqual(GameStateManager.GameState.Round_Battle, gsm.currentGameState);
    }

    [TearDown]
    public void ResetScene()
    {
        TestReseter.TearDownScene();
    }
}
