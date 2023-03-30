using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages and keeps track of the game state as the user progresses through each round or tutorial.
/// </summary>
public class GameStateManager : MonoBehaviour
{   
    /// <summary>
    /// TextMeshPro Game Object to display UI elements on.
    /// </summary>
    public TMP_Text InfoText;
    /// <summary>
    /// The button GameObject in the scene which should call the BeginBattle() method.
    /// </summary>
    public GameObject BeginBattleButton;
    /// <summary>
    /// A Debug wrapper object which will be enabled when playing in the Unity Editor.
    /// </summary>
    public GameObject DebugObject;
    /// <summary>
    /// 
    /// </summary>
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
            DebugObject.SetActive(true);
        }
        currentState = GameState.Calibration;
        UIController = GetComponent<UIController>();
        qrDetection = GetComponent<QRDetection>();
        enemyManager = GetComponent<EnemyManager>();
        roundTimer = GetComponent<RoundTimer>();
        InfoText.text = "";
        UIController.CalibrationPopUp();
    }

    private void Update()
    {
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
        InfoText.transform.position = GameProperties.Centre + new Vector3(0, 0.7f, 0);
        currentState = GameState.Round_Plan;
        roundNumber++;
        InfoText.text = "Round " + roundNumber.ToString() + "\n-[Planning]-";
        BeginBattleButton.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        BeginBattleButton.SetActive(true);

    }

    public void BeginBattle()
    {
        InfoText.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        qrDetection.StopQR();
        enemyManager.StartSpawning(enemyWaves[roundNumber]);
        roundTimer.StartTimer();
        currentState = GameState.Round_Battle;

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
            InfoText.text = "Round " + roundNumber.ToString() + " Over\n-[" + enemiesKilled.ToString() + " Enemies Killed]-";
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
