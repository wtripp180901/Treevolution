using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

public class UIController : MonoBehaviour
{
    public GameObject infoDialogPrefab;
    public GameObject buttonDialogPrefab;

    public Material backPlateGrey;
    public Material backPlateOrange;
    public Material backPlateGreen;

    public TMP_Text infoText;
    public int roundTime = 60;
    private EnemyManager enemyManager;
    private List<Dialog> openDialogs = new List<Dialog>();


    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
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
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.Close, "Calibration Success", "You have successfully calibrated the Game Board! Ensure that it lines up with the table, otherwise adjust.", true);
        d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateGreen;
        openDialogs.Add(d);
    }

    private void closeOpenDialogs()
    {
        for (int i = 0; i < openDialogs.Count; i++)
        {
            openDialogs[i].DismissDialog();
        }
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

    public void DecreaseTime()
    {
        infoText.text = "" + roundTime;
        roundTime -= 1;
    }
}

