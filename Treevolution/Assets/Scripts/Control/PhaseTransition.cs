using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PhaseTransition : MonoBehaviour
{
    public TMP_Text infoText;
    public GameObject tree;
    public GameObject startButton;
    public GameObject debugObject;
    public bool devMode = false;
    private UIController UIController;
    private PlaneMapper planeMapper;
    private GameState currentState;
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
    private bool firstRun = true;

    [HideInInspector]
    public UnityEvent planeMapped;

    private void Start()
    {
        if (devMode)
        {
            startButton.SetActive(true);
            debugObject.SetActive(true);
        }
        currentState = GameState.Calibration;
        UIController = GetComponent<UIController>();
        planeMapper = GetComponent<PlaneMapper>();

        planeMapped.AddListener(PlaneMappedHandler);



        infoText.text = "";
        UIController.CalibrationPopUp();
    }

    void PlaneMappedHandler()
    {
        currentState = GameState.Plane_Mapped;
        UIController.CalibrationSuccessPopUp();
        //infoText.text = "Calibration Successful";
    }

    public void GoToGamePhase()
    {
        if (devMode)
        {
            GameObject debugQR = GameObject.Find("DebugQR");
            PlaneMapper planeMapper = gameObject.GetComponent<PlaneMapper>();
            Pose newPose = new Pose(debugQR.transform.position, debugQR.transform.rotation);
            //newPose.rotation = Quaternion.LookRotation(newPose.up, newPose.forward);
            //newPose.rotation.eulerAngles = newPose.rotation.eulerAngles + new Vector3(90, 15, 20);
            planeMapper.CreateNewPlane(newPose);//, new Vector3(-1.063f, -0.392f, 2.13f));
        }
        GetComponent<RealWorldPropertyMapper>().MapProperties();
        // Game Start Animation?
        GetComponent<EnemyManager>().StartSpawning();
        GetComponent<QRDetection>().StopQR();
        GetComponent<RoundTimer>().PauseTimer();
        Destroy(GameObject.FindWithTag("NextRoundButton"));
    }

    public void GameOverScreen(bool win)
    {
        GetComponent<RoundTimer>().StopTimer();
        GetComponent<EnemyManager>().StopSpawning();
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = 0;i < enemyList.Length; i++)
        {
            Destroy(enemyList[i]);
        }
        if (win)
        {
            GetComponent<UIController>().Win();
        }else
        {
            GetComponent<UIController>().Lose();
        }
    }
}
