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
        GameStateManager gsm = new GameStateManager();
        gsm.SetupGameStateManagerTesting();
        GameProperties.SetTestProperties(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.one, Vector3.zero, Pose.identity, 0);
        Assert.AreEqual(GameStateManager.GameState.Calibration, gsm.currentGameState);
        
        // Calibration Success
        try
        {
            gsm.CalibrationSuccess();
        }
        catch (System.ArgumentException e)
        { }
        Assert.AreEqual(GameStateManager.GameState.Calibration_Success, gsm.currentGameState);
        
        // Tutorial Plan
        try
        {
            gsm.BeginTutorialPlan();
        }
        catch (System.ArgumentException e)
        { }
        Assert.AreEqual(GameStateManager.GameState.Tutorial_Plan, gsm.currentGameState);

        // Tutorial Battle
        try
        {
            gsm.BeginBattle();
        }
        catch (System.NullReferenceException e)
        { }
        Assert.AreEqual(GameStateManager.GameState.Tutorial_Battle, gsm.currentGameState);

        // Round Plan
        try
        {
            gsm.BeginRound();
        }
        catch (System.NullReferenceException e)
        { }
        Assert.AreEqual(GameStateManager.GameState.Round_Plan, gsm.currentGameState);
        
        // Round Battle
        try
        {
            gsm.BeginBattle();
        }
        catch (System.NullReferenceException e)
        { }
        Assert.AreEqual(GameStateManager.GameState.Round_Battle, gsm.currentGameState);
    }
}
