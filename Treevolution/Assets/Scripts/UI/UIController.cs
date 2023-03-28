using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Threading;

public class UIController : MonoBehaviour
{
    public GameObject infoDialogPrefab;
    public GameObject buttonDialogPrefab;
    public GameObject buttonImageDiaglogPrefab;

    public Sprite antImage;
    public Sprite armouredBeetleImage;
    public Sprite armouredCockroachImage;
    public Sprite armouredStegbeetleImage;
    public Sprite dragonflyImage;
    public Sprite hornetImage;

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
        closeOpenDialogs();
        Dialog d = Dialog.Open(infoDialogPrefab, DialogButtonType.None, "Calibrate Game Board", "Find, and look at the QR Code in the corner of the table to calibrate the Game Board.", true);
        d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateOrange;
        lock (openDialogs)
        {
            openDialogs.Add(d);
        }
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
        lock (openDialogs)
        {
            openDialogs.Add(d);
        }
    }

    private void TutorialSelectionPopUp()
    {
        closeOpenDialogs();
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.Yes | DialogButtonType.No, "Begin Tutorial", "Would you like to proceed with the tutorial level?", true);
        d.OnClosed += HandleTutorialSelectionEvent;// delegate (DialogResult dr) { TutorialSelectionPopUp(); };
        lock (openDialogs)
        {
            openDialogs.Add(d);
        }
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

    private void WaitForDialogClosed(Dialog d)
    {
        while (d.State != DialogState.Closed)
        {
            Thread.Sleep(100);
        }
    }

    public void TutorialPlanPopUps()
    {
        closeOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial", "You must protect your Home Tree from the enemy bugs! Some bugs you can simply whack, whereas others you must use the help of your plant buddies.", true);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Planning Phase", "The enemy bugs will come in waves from either far end of the table. Place your obstacles and plants strategically during each Planning Phase to maximise your score.", true);
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Planning Phase", "Have a go, place your objects where you would like, and when you are happy then press the big red button above your Home Tree in the centre.", true);
                d3.OnClosed = delegate (DialogResult dr)
                {
                    gameStateManager.BeginTutorialPlan();
                };
            };
        };
    }

    public void TutorialBattlePopUps()
    {
        closeOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Battle Phase", "During the Battle Phase the time remaining is displayed above your Home Tree.", true);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Bugs", "You will need to keep an eye out for new bugs spawning. There are various types of bugs in fact, starting with the most basic - Ants.", true);
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Ants", "Ants can be damaged by *whacking* them, *splatting* them, *swatting* them, or any other similar action. It may take a few tries to get used to, but have a go!", true);
                d3.gameObject.GetComponentInChildren<Image>().sprite = antImage;
                d3.OnClosed = delegate (DialogResult dr)
                {
                    gameStateManager.TutorialBattlePlay();
                };
            };
        };
    }

    public void TutorialBugPopUps()
    {
        closeOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Bugs", "More types of bugs will be introduced as you progress through the rounds. Some of them can only be damaged using your plants, and others can even break down obstacles that you place!", true);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Beetle", "The Beetle is slightly stronger than the ant, but its armour slows it down. It can be damaged by hitting it, although it isn't a one-hit-wonder like with the ant. Alternatively target it with your towers.", true);
            d2.gameObject.GetComponentInChildren<Image>().sprite = armouredBeetleImage;
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Cockroach", "The Cockroach is strong and fast! Be sure to target these when you first see them, and don't let them get too close to your Home Tree. Their armour is so strong that you cannot damage them with pure force - plant power must be used.", true);
                d3.gameObject.GetComponentInChildren<Image>().sprite = armouredCockroachImage;
                d3.OnClosed = delegate (DialogResult dr)
                {
                    Dialog d4 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Stagbeetle", "The Stagbeetle may seem slow, but its health and attack damage sure does make up for it. They can break through obstacles with a few hits, and have a large amount health.", true);
                    d4.gameObject.GetComponentInChildren<Image>().sprite = armouredStegbeetleImage;
                    d4.OnClosed = delegate (DialogResult dr)
                    {
                        Dialog d5 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Dragonfly", "These beautiful creatures can fly straight over your obstacles and will deal a large amount of damage to your Home Tree, so be sure to swat them when you see them.", true);
                        d5.gameObject.GetComponentInChildren<Image>().sprite = dragonflyImage;
                        d5.OnClosed = delegate (DialogResult dr)
                        {
                            Dialog d6 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.Confirm, "Treevolution Tutorial - Hornet", "I'd stay away from these if I were you - swatting will just result in a painful sting! Instead, use your plants to damage these pests!", true);
                            d6.gameObject.GetComponentInChildren<Image>().sprite = hornetImage;
                            d6.OnClosed = delegate (DialogResult dr)
                            {
                                gameStateManager.TutorialBattleContinue();
                            };
                        };
                    };
                };
            };
        };
    }

    public void EndPopUp()
    {
        closeOpenDialogs();
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Congratulations!", "Score: " + enemyManager.getEnemiesKilled(), true);
        d.OnClosed += delegate (DialogResult dr)
        {
            SceneManager.LoadScene("StartMenu");
            SceneManager.UnloadSceneAsync("Game");
        };
        lock (openDialogs)
        {
            openDialogs.Add(d);
        }
    }

    private void closeOpenDialogs()
    {
        if (openDialogs != null)
        {
            lock (openDialogs)
            {
                for (int i = 0; i < openDialogs.Count; i++)
                {
                    openDialogs[i].DismissDialog();
                }
                openDialogs.Clear();
            }
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

