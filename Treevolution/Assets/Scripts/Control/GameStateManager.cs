using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    public TMP_Text infoText;
    public TMP_Text debugText;
    public GameObject tree;
    public GameObject startButton;
    public GameObject debugObject;
    public bool devMode = false;
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
        Dragonfly
    }
    private Dictionary<EnemyType, int>[] enemyWaves = {
        new Dictionary<EnemyType, int>(){ 
            { EnemyType.Ant, 10 } 
        },
        new Dictionary<EnemyType, int>(){
            { EnemyType.Ant, 10 },
            { EnemyType.Armoured_Bug, 5 }
        },
        new Dictionary<EnemyType, int>(){
            { EnemyType.Ant, 10 },
            { EnemyType.Armoured_Bug, 8 },
            { EnemyType.Armoured_Cockroach, 5}
        },
        new Dictionary<EnemyType, int>(){
            { EnemyType.Ant, 10 },
            { EnemyType.Armoured_Bug, 8 },
            { EnemyType.Armoured_Cockroach, 5},
            { EnemyType.Armoured_Stagbeetle, 5 }
        },
        new Dictionary<EnemyType, int>(){
            { EnemyType.Ant, 10 },
            { EnemyType.Armoured_Bug, 8 },
            { EnemyType.Armoured_Cockroach, 5},
            { EnemyType.Armoured_Stagbeetle, 5 },
            { EnemyType.Dragonfly, 3}
        }
    };

    private void Start()
    {
        if (devMode)
        {
            debugObject.SetActive(true);
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
        UIController.TutorialPlanPopUp();
        currentState = GameState.Tutorial_Battle;
        UIController.TutorialBattlePopUp();
    }


    public void BeginRound()
    {
        infoText.transform.position = GameProperties.Centre + new Vector3(0, 0.7f, 0);
        currentState = GameState.Round_Plan;
        roundNumber++;
        infoText.text = "Round " + roundNumber.ToString() + "\n-[Planning]-";
        startButton.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        startButton.SetActive(true);
        debugText.text = "paused = " + roundTimer.IsPaused.ToString() + "\n" + "firstSpawn = " + enemyManager.firstSpawn + "\nspawning = " + enemyManager.started;

    }

    public void BeginBattle()
    {
        infoText.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        qrDetection.StopQR();
        enemyManager.StartSpawning(enemyWaves[roundNumber]);
        roundTimer.StartTimer();
        currentState = GameState.Round_Battle;
        debugText.text = "paused = " + roundTimer.IsPaused.ToString() + "\n" + "firstSpawn = " + enemyManager.firstSpawn + "\nspawning = " + enemyManager.started;

    }

    public void EndBattle()
    {
        try
        {
            clearEnemies();
            roundTimer.StopTimer();
            enemyManager.StopSpawning();
            qrDetection.StartQR();
            int enemiesKilled = GetComponent<EnemyManager>().getEnemiesKilled();
            infoText.text = "Round " + roundNumber.ToString() + " Over\n-[" + enemiesKilled.ToString() + " Enemies Killed]-";
            StartCoroutine(displayScore(3));
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void EndGame()
    {
        UIController.EndPopUp();
    }

    IEnumerator displayScore(int secs)
    {
        yield return new WaitForSeconds(secs);
        if (roundNumber <= 3)
        {
            BeginRound();
        }
        else
        { 
            EndGame();
        }
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
