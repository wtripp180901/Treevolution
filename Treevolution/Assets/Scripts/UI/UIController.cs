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
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

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
    private int roundTime = -1;
    public int timeRemaining { get { return roundTime; } }
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
        lock (openDialogs)
        {
            Dialog d = Dialog.Open(infoDialogPrefab, DialogButtonType.None, "Calibrate Game Board", "Find, and look at the QR Code in the corner of the table to calibrate the Game Board.", true);
            d.gameObject.GetComponent<Follow>().enabled = false;
            d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateOrange;
            openDialogs.Add(d);
        }
    }

    public void CalibrationSuccessPopUp()
    {
        closeOpenDialogs();
        lock (openDialogs)
        {
            Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Calibration Success", "You have successfully calibrated the Game Board! Ensure that it lines up with the table, then click Confirm to lock the board in place.", true);
            d.gameObject.GetComponent<Follow>().enabled = false;
            d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateGreen;
            d.OnClosed = delegate (DialogResult dr)
            {
                GetComponent<QRDetection>().lockPlane = true;
                GetComponent<RealWorldPropertyMapper>().MapProperties();
                TutorialSelectionPopUp();
            };
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

    public void TutorialPlanPopUps()
    {
        closeOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Treevolution Tutorial", "Protect your Home Tree from the enemy bugs! Some bugs you can simply squash, whereas others you must utilise the help of your plant buddies.", true);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Planning Phase", "The enemy bugs will come in waves from either end of the table. Place your obstacles and plants during each Planning Phase, try to maximise your strategy and learn from previous mistakes.", true);
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Give it a Try", "Have a go, place your objects where you would like, and when you are happy, press the big red button above your Home Tree in the centre.\nBring On The Bugs!", true);
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
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Battle Phase", "During the Battle Phase the time remaining is displayed above your Home Tree. Kill as many bugs as you can before the time runs out!", true);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Bugs", "Keep an eye out for new bugs approaching, the most basic of which is the Ant.", true);
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Ants", "Ants can be killed by *splatting* them, and is easiest with your hand in a fist. It may take a few tries to get used to, but have a go!", true);
                d3.gameObject.GetComponentInChildren<Image>().sprite = antImage;
                d3.OnClosed = delegate (DialogResult dr)
                {
                    StartCoroutine(gameStateManager.BeginTutorialBattle());
                };
            };
        };
    }

    public void TutorialBugPopUps()
    {
        closeOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Bugs", "More types of bugs will be introduced as you progress through the rounds. Some of them can only be damaged using your plants, and others can even break down obstacles that you place!", true);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Beetles", "The Beetle is stronger than the ant, but its armour slows it down. It can also be damaged by hitting it, although may take a few strikes. Alternatively target it with your plants.", true);
            d2.gameObject.GetComponentInChildren<Image>().sprite = armouredBeetleImage;
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Cockroaches", "The Cockroach is strong and fast. Their armour is so strong that you cannot damage them with pure force - plant power must be used.", true);
                d3.gameObject.GetComponentInChildren<Image>().sprite = armouredCockroachImage;
                d3.OnClosed = delegate (DialogResult dr)
                {
                    Dialog d4 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Stagbeetles", "The Stagbeetle may seem slow, but its health and attack damage makes up for it. They can break through obstacles with a few hits, and have a large amount health.", true);
                    d4.gameObject.GetComponentInChildren<Image>().sprite = armouredStegbeetleImage;
                    d4.OnClosed = delegate (DialogResult dr)
                    {
                        Dialog d5 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Dragonflies", "These beautiful creatures can fly straight over your obstacles and will deal a large amount of damage to your Home Tree, so be sure to swat them when you see them.", true);
                        d5.gameObject.GetComponentInChildren<Image>().sprite = dragonflyImage;
                        d5.OnClosed = delegate (DialogResult dr)
                        {
                            Dialog d6 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Hornets", "I'd stay away from these if I were you - swatting will just result in a painful sting - instead, use your plants!", true);
                            d6.gameObject.GetComponentInChildren<Image>().sprite = hornetImage;
                            d6.OnClosed = delegate (DialogResult dr)
                            {
                                gameStateManager.ContinueTutorialBattle();
                            };
                        };
                    };
                };
            };
        };
    }

    public void EndTutorial()
    {
        closeOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Tutorial Complete", "Now it's time to start for real. Strategically place your items, and click the red button when you are ready to begin the first battle!", true);
        d1.OnClosed = delegate (DialogResult dr)
        {
            gameStateManager.BeginRound();
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

