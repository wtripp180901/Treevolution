using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    public TMP_Text infoText;
    public GameObject tree;
    public GameObject startButton;
    public GameObject debugObject;
    public bool devMode = false;
    private UIController UIController;
    private GameState currentState;
    private int roundNumber = 0;

    private enum GameState
    {
        Calibration,
        Plane_Mapped,
        Tutorial_Plan,
        Tutorial_Battle,
        Round1_Plan,
        Round1_Battle,
        Round2_Plan,
        Round2_Battle,
        Round3_Plan,
        Round3_Battle
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
        currentState = GameState.Tutorial_Plan;
        UIController.TutorialPlanPopUp();
        currentState = GameState.Tutorial_Battle;
        UIController.TutorialBattlePopUp();
    }


    public void BeginRound()
    {
        currentState++;
        roundNumber++;
        infoText.text = "Round " + roundNumber.ToString() + " - Planning";
        startButton.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        startButton.SetActive(true);
    }

    public void BeginBattle()
    {
        infoText.text = "";
        GetComponent<QRDetection>().StopQR();
        GetComponent<EnemyManager>().StartSpawning(enemyWaves[roundNumber-1]);
        GetComponent<RoundTimer>().StartTimer();
    }

    public void EndBattle()
    {
        try
        {
            clearEnemies();
            GetComponent<RoundTimer>().StopTimer();
            GetComponent<EnemyManager>().StopSpawning();
            GetComponent<QRDetection>().StartQR();
            startButton.SetActive(true);
            int enemiesKilled = GetComponent<EnemyManager>().getEnemiesKilled();
            infoText.text = "Round " + roundNumber.ToString() + " Over\n" + enemiesKilled.ToString() + " Enemies Killed";
            StartCoroutine(waitAndNextRound(3));
        }
        catch(UnityException e)
        {
            Debug.Log(e.Message);
        }
    }

    IEnumerator waitAndNextRound(int secs)
    {
        yield return new WaitForSeconds(secs);
        BeginRound();
    }
    

    private void clearEnemies()
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyList.Length; i++)
        {
            Destroy(enemyList[i]);
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
