using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using System;

public class GameStateManager : MonoBehaviour
{
    public TMP_Text infoText;
    public TMP_Text debugText;
    public GameObject tree;
    public GameObject startButton;
    public GameObject debugObject;
    public int maxRoundNum = 4;
    private QRDetection qrDetection;
    private UIController UIController;
    private EnemyManager enemyManager;
    private RoundTimer roundTimer;
    private GameState currentState;
    public GameState CurrentGameState { get { return currentState; } }
    private int roundNumber = 0;

    public enum GameState
    {
        Calibration,
        Plane_Mapped,
        Tutorial_Plan,
        Tutorial_Battle,
        Round_Plan,
        Round_Battle
    }
    public enum EnemyType
    {
        Ant,
        Armoured_Bug,
        Armoured_Cockroach,
        Armoured_Stagbeetle,
        Dragonfly,
        Hornet
    }
    private Dictionary<EnemyType, int>[] enemyWaves = {
        new Dictionary<EnemyType, int>(){
            { EnemyType.Ant, 10 },
        },
        new Dictionary<EnemyType, int>(){
            { EnemyType.Ant, 10 },
            { EnemyType.Armoured_Bug, 10 }
        },
        new Dictionary<EnemyType, int>(){
            { EnemyType.Ant, 10 },
            { EnemyType.Armoured_Bug, 8 },
            { EnemyType.Armoured_Cockroach, 8}
        },
        new Dictionary<EnemyType, int>(){
            { EnemyType.Ant, 8 },
            { EnemyType.Armoured_Bug, 8 },
            { EnemyType.Armoured_Cockroach, 8},
            { EnemyType.Armoured_Stagbeetle, 5 },
            { EnemyType.Dragonfly, 5},

        },
        new Dictionary<EnemyType, int>(){
            { EnemyType.Ant, 8 },
            { EnemyType.Armoured_Bug, 5 },
            { EnemyType.Armoured_Cockroach, 8},
            { EnemyType.Armoured_Stagbeetle, 5 },
            { EnemyType.Dragonfly, 5},
            { EnemyType.Hornet, 5}
        }
    };

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            debugObject.SetActive(true); // Dev mode
        }
        currentState = GameState.Calibration;
        UIController = GetComponent<UIController>();
        qrDetection = GetComponent<QRDetection>();
        enemyManager = GetComponent<EnemyManager>();
        roundTimer = GetComponent<RoundTimer>();
        infoText.text = "";
        UIController.CalibrationPopUp();
    }

    public void CalibrationSuccess()
    {
        currentState = GameState.Plane_Mapped;
        UIController.CalibrationSuccessPopUp();
        //infoText.text = "Calibration Successful";
    }


    public void BeginTutorial()
    {
        roundNumber = 0;
        currentState = GameState.Tutorial_Plan;
        UIController.TutorialPlanPopUps();
    }


    public void BeginTutorialPlan()
    {
        infoText.transform.position = GameProperties.Centre + new Vector3(0, 0.65f, 0);
        currentState = GameState.Tutorial_Plan;
        infoText.text = "Tutorial\n[Planning]";
        startButton.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        startButton.SetActive(true);
    }

    private IEnumerator BeginTutorialBattleInfo()
    {
        currentState = GameState.Tutorial_Battle;
        roundTimer.SetRoundLength(30);
        enemyManager.StartSpawning(enemyWaves[0]);
        roundTimer.StartTimer(); // play
        yield return StartCoroutine(pause(1));
        roundTimer.PauseTimer();
        UIController.TutorialBattlePopUps();
    }

    public IEnumerator BeginTutorialBattle()
    {
        roundTimer.PauseTimer(); // play
        yield return new WaitUntil(() => enemyManager.getEnemiesKilled() != 0);
        roundTimer.PauseTimer(); // pause
        UIController.TutorialBugPopUps();
    }
    public void ContinueTutorialBattle()
    {
        roundTimer.PauseTimer();
    }

    private IEnumerator EndTutorialBattle(int enemiesKilled)
    {
        enemyManager.resetEnemiesKilled();
        infoText.text = "Tutorial Over\n[" + enemiesKilled.ToString() + " Enemies Killed]";
        yield return StartCoroutine(pause(3));
        UIController.EndTutorial();
    }

    public void BeginRound()
    {
        roundTimer.SetRoundLength(60);
        infoText.transform.position = GameProperties.Centre + new Vector3(0, 0.65f, 0);
        currentState = GameState.Round_Plan;
        roundNumber++;
        infoText.text = "Round " + roundNumber.ToString() + "\n[Planning]";
        startButton.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        startButton.SetActive(true);
    }

    public void BeginBattle()
    {
        startButton.SetActive(false);
        infoText.transform.position = GameProperties.Centre + new Vector3(0, 0.5f, 0);
        qrDetection.StopQR();
        if (currentState == GameState.Tutorial_Plan)
        {
            StartCoroutine(BeginTutorialBattleInfo());
            return;
        }
        enemyManager.StartSpawning(enemyWaves[roundNumber]);
        roundTimer.StartTimer();
        currentState = GameState.Round_Battle;

    }

    public IEnumerator EndBattle()
    {
        clearEnemies();
        enemyManager.StopSpawning();
        qrDetection.StartQR();
        int enemiesKilled = GetComponent<EnemyManager>().getEnemiesKilled();
        if(CurrentGameState == GameState.Tutorial_Battle)
        {
            StartCoroutine(EndTutorialBattle(enemiesKilled));
            yield break;
        }
        infoText.text = "Round " + roundNumber.ToString() + " Over\n[" + enemiesKilled.ToString() + " Enemies Killed]";
        yield return StartCoroutine(pause(3));
        if (roundNumber < maxRoundNum)
        {
            BeginRound();
        }
        else
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        UIController.EndPopUp();
    }

    IEnumerator pause(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    

    private void clearEnemies()
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyList.Length; i++)
        {
            enemyList[i].GetComponent<EnemyScript>().DestroyEnemy(false);
        }
    }

    public void GameOverScreen(bool win)
    {
        clearEnemies();
        if (win)
        {
            GetComponent<UIController>().Win();
        }else
        {
            GetComponent<UIController>().Lose();
        }
    }
}
