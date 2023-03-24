using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit;

public class UIController : MonoBehaviour
{
    public GameObject infoDialogPrefab;
    public GameObject buttonDialogPrefab;

    public Material backPlateGrey;
    public Material backPlateOrange;
    public Material backPlateGreen;

    public TMP_Text infoText;
    private int roundTime = 60;
    private EnemyManager enemyManager;
    private GameStateManager gameStateManager;
    private List<Dialog> openDialogs = new List<Dialog>();


    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        gameStateManager = GetComponent<GameStateManager>();
    }

    public void CalibrationPopUp()
    {
        Dialog d = Dialog.Open(infoDialogPrefab, DialogButtonType.None, "Calibrate Game Board", "Find, and look at the QR Code in the corner of the table to calibrate the Game Board.", true);
        d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateOrange;
        openDialogs.Add(d);
    }

    public void CalibrationSuccessPopUp()
    {
        closeOpenDialogs();
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Calibration Success", "You have successfully calibrated the Game Board! Ensure that it lines up with the table, then click Confirm to lock the board in place.", true);
        d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateGreen;
        d.OnClosed = delegate (DialogResult dr) {
            GetComponent<QRDetection>().lockPlane = true;
            GetComponent<RealWorldPropertyMapper>().MapProperties();
            TutorialSelectionPopUp(); 
        };
        openDialogs.Add(d);
    }

    private void TutorialSelectionPopUp()
    {
        closeOpenDialogs();
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.Yes | DialogButtonType.No, "Begin Tutorial", "Would you like to proceed with the tutorial level?", true);
        d.OnClosed += HandleTutorialSelectionEvent;// delegate (DialogResult dr) { TutorialSelectionPopUp(); };
        openDialogs.Add(d);
    }

    private void HandleTutorialSelectionEvent(DialogResult result)
    {
        if (result.Result == DialogButtonType.Yes)
        {
            gameStateManager.BeginTutorial();
        }
        else
        {
            gameStateManager.BeginRound();
        }
    }

    public void TutorialPlanPopUp()
    {

    }

    public void TutorialBattlePopUp()
    {

    }

    public void EndPopUp()
    {
        closeOpenDialogs();
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Congratulations!", "Score: " + enemyManager.getEnemiesKilled(), true);
        d.OnClosed += delegate (DialogResult dr)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartMenu");
        };
        openDialogs.Add(d);
    }

    private void closeOpenDialogs()
    {
        for (int i = 0; i < openDialogs.Count; i++)
        {
            openDialogs[i].DismissDialog();
        }
        openDialogs.Clear();
    }

    public void Win()
    {
        string enemiesKilled = enemyManager.getEnemiesKilled().ToString() ;
        infoText.text = "You win!\nEnemies Killed: " + enemiesKilled;
    }

    public void Lose()
    {
        string enemiesKilled = enemyManager.getEnemiesKilled().ToString();
        infoText.text = "You lose!\nEnemies Killed: " + enemiesKilled;
    }

    public void ResetTimer(int time)
    {
        roundTime = time;
    }

    public void DecreaseTime()
    {
        roundTime -= 1;
        infoText.text = "" + roundTime.ToString();
    }
}

