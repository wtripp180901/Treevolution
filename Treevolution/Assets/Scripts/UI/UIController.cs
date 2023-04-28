using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Controls the instantiation and removal of UI elements.
/// </summary>
public class UIController : MonoBehaviour
{
    /// <summary>
    /// Adapted MRTK Dialog Prefab to display only text with no buttons or images.
    /// </summary>
    public GameObject infoDialogPrefab;
    /// <summary>
    /// Adapted MRTK Dialog Prefab to display text and button(s).
    /// </summary>
    public GameObject buttonDialogPrefab;
    /// <summary>
    /// Adapted MRTK Dialog Prefab to display text, an image, and button(s).
    /// </summary>
    public GameObject buttonImageDiaglogPrefab;

    /// <summary>
    /// Gif of hitting bug.
    /// </summary>
    public Sprite damageGif;
    /// <summary>
    /// Image of ant enemy.
    /// </summary>
    public Sprite antImage;
    /// <summary>
    /// Image of beetle enemy.
    /// </summary>
    public Sprite armouredBeetleImage;
    /// <summary>
    /// Image of cockroach enemy.
    /// </summary>
    public Sprite armouredCockroachImage;
    /// <summary>
    /// Image of stagbeetle enemy.
    /// </summary>
    public Sprite armouredStegbeetleImage;
    /// <summary>
    /// Image of dragonfly enemy.
    /// </summary>
    public Sprite dragonflyImage;
    /// <summary>
    /// Image of hornet enemy.
    /// </summary>
    public Sprite hornetImage;

    /// <summary>
    /// Material for a grey dialog pop-up.
    /// </summary>
    public Material backPlateGrey;
    /// <summary>
    /// Material for an orange dialog pop-up.
    /// </summary>
    public Material backPlateOrange;
    /// <summary>
    /// Material for a green dialog pop-up.
    /// </summary>
    public Material backPlateGreen;

    /// <summary>
    /// TextMeshPro Game Object to display UI elements on.
    /// </summary>
    public TMP_Text infoText;

    public TMP_Text dictationText;
	
    int time = 60;
	
    private EnemyManager enemyManager;
	
    /// <summary>
    /// Gets the time remaining in the current round.
    /// </summary>
    public int timeRemaining { get { return _roundTime; } }

    /// <summary>
    /// The time remaining in the current round.
    /// </summary>
    private int _roundTime = -1;
    /// <summary>
    /// Running EnemyManager instance.
    /// </summary>
    private EnemyManager _enemyManager;
    /// <summary>
    /// Running GameStateManager instance.
    /// </summary>
    private GameStateManager _gameStateManager;
    /// <summary>
    /// List of currently open dialogs.
    /// </summary>
    private List<Dialog> _openDialogs = new List<Dialog>();

    /// <summary>
    /// Start runs when loading the GameObject that this script is attached to.
    /// </summary>
    private void Start()
    {
        _enemyManager = GetComponent<EnemyManager>();
        _gameStateManager = GetComponent<GameStateManager>();
    }

    public void SetupForTest()
    {
        _enemyManager = GetComponent<EnemyManager>();
        _gameStateManager = GetComponent<GameStateManager>();
        infoDialogPrefab = new GameObject();
    }

    /// <summary>
    /// Displays an orange info pop-up requesting calibration of the game board.
    /// </summary>
    public void CalibrationPopUp()
    {
        CloseOpenDialogs();
        lock (_openDialogs)
        {
            if (_gameStateManager.currentGameState != GameStateManager.GameState.Calibration_Success)
            {
                Dialog d = Dialog.Open(infoDialogPrefab, DialogButtonType.None, "Calibrate Game Board", "Find, and look at the QR Code in the corner of the table to calibrate the Game Board.", true);
                d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateOrange;
                _openDialogs.Add(d);
            }
        }
    }

    /// <summary>
    /// Displays a green button pop-up displaying calibration success, and continuing to Tutorial selection when it is closed.
    /// </summary>
    public void CalibrationSuccessPopUp()
    {
        CloseOpenDialogs();
        lock (_openDialogs)
        {
            Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Calibration Success", "You have successfully calibrated the Game Board! Ensure that it lines up with the table, then click Confirm to lock the board in place.", true);
            d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateGreen;
            d.OnClosed = delegate (DialogResult dr)
            {
                GetComponent<QRDetection>().lockPlane = true;
                GetComponent<RealWorldPropertyMapper>().MapProperties();
                //GameObject.FindGameObjectWithTag("Floor").GetComponent<Area>().GenerateObjectsByInfo(); //THIS LINE ENABLES GRASS DRAWING
                TutorialSelectionPopUp();
            };
            _openDialogs.Add(d);
        }
    }

    /// <summary>
    /// Displays a button pop-up asking the user to select whether they would like to start the Tutorial.
    /// </summary>
    private void TutorialSelectionPopUp()
    {
        CloseOpenDialogs();
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.Yes | DialogButtonType.No, "Begin Tutorial", "Would you like to proceed with the tutorial level?", true);
        d.OnClosed += HandleTutorialSelectionEvent;
        lock (_openDialogs)
        {
            _openDialogs.Add(d);
        }
    }

    /// <summary>
    /// Handles the users choice of Tutorial selection.
    /// </summary>
    /// <param name="result">DialogResult object returned from the closed dialog selection pop-up.</param>
    private void HandleTutorialSelectionEvent(DialogResult result)
    {
        if (result.Result == DialogButtonType.Yes)
        {
            _gameStateManager.BeginTutorial();
        }
        else
        {
            _gameStateManager.BeginRound();
        }
    }

    /// <summary>
    /// Displays a number of sequential tutorial pop-ups containing information about the planning phase, the last of which initiates the tutorial planning phase on closure.
    /// </summary>
    public void TutorialPlanPopUps()
    {
        CloseOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Treevolution Tutorial", "Protect your Home Tree from the enemy bugs! You can either use your hands to damage the bugs, or utilise the help of your plants and bird buddy.", true);
        (Vector3 position, Quaternion rotation) t = (d1.transform.position, d1.transform.rotation);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Planning Phase", "Each round, the enemy bugs will come from either far end of the battleground. Place your obstacles and plants during each Planning Phase to optimise your strategy; learn from past mistakes.", true);
            d2.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Moving Obstacles", "If you are not happy with your obstacle placement during battle, you can still move your obstacles. However, those obstacles will become disabled for a short period of time as a punishment.", true);
                d3.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                d3.OnClosed = delegate (DialogResult dr)
                {
                    Dialog d4 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Give it a Try", "Have a go, place your items where you would like, and when you are happy, press the big red button above your Home Tree in the centre.\nBring On The Bugs!", true);
                    d4.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                    d4.OnClosed = delegate (DialogResult dr)
                    {
                        _gameStateManager.BeginTutorialPlan();
                    };
                };
            };
            t = (d2.transform.position, d2.transform.rotation);
        };
        t = (d1.transform.position, d1.transform.rotation);
    }

    /// <summary>
    /// Displays a number of sequential tutorial pop-ups containing information about the battle phase, the last of which initiates the tutorial battle phase on closure.
    /// </summary>
    public void TutorialBattlePopUps()
    {
        CloseOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Battle Phase", "The time remaining is displayed above your Home Tree. Kill as many bugs as you can before the time runs out!", true);
        (Vector3 position, Quaternion rotation) t = (d1.transform.position, d1.transform.rotation);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Bugs", "Keep an eye out for new bugs approaching, the most basic of which is the Ant.", true);
            d2.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Ants", "Ants are the weakest of the bugs. They have no armour, and can be squashed rather easily.", true);
                d3.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                d3.gameObject.GetComponentInChildren<Image>().sprite = antImage;
                d3.OnClosed = delegate (DialogResult dr)
                {
                    Dialog d4 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Damage", "Most bugs can be damaged by *whacking* them. This works best by hitting the table just in front of the bug with your hand flat, or in a fist.", true);
                    d4.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                    d4.gameObject.GetComponentInChildren<Image>().sprite = damageGif;
                    d4.OnClosed = delegate (DialogResult dr)
                    {
                        StartCoroutine(_gameStateManager.BeginTutorialBattle());
                    };
                };
                t = (d3.transform.position, d3.transform.rotation);
            };
            t = (d2.transform.position, d2.transform.rotation);
        };
        t = (d1.transform.position, d1.transform.rotation);
    }

    /// <summary>
    /// Displays a number of sequential tutorial pop-ups containing information about the bug types, the last of which continues the tutorial battle phase on closure.
    /// </summary>
    public void TutorialBugPopUps()
    {
        CloseOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Bugs", "More types of bugs will be introduced as you progress through the rounds. Be mindful of which ones you *whack*, though, as some have strong armour and it may make more sense to target these with your plants.", true);
        (Vector3 position, Quaternion rotation) t = (d1.transform.position, d1.transform.rotation);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Beetles", "The Beetle is stronger than the ant, but its armour slows it down. It takes a few strikes to kill by hand, or alternatively target it with your plants or buddy.", true);
            d2.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
            d2.gameObject.GetComponentInChildren<Image>().sprite = armouredBeetleImage;
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Cockroaches", "The Cockroach is strong and fast. Their armour will take long to break with pure force. Consider using your buddy by saying something like 'Attack the Cockroaches'.", true);
                d3.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                d3.gameObject.GetComponentInChildren<Image>().sprite = armouredCockroachImage;
                d3.OnClosed = delegate (DialogResult dr)
                {
                    Dialog d4 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Stagbeetles", "The Stagbeetle may seem slow, but its health and attack damage makes up for it. They can break through obstacles with a few hits, and have a large amount health.", true);
                    d4.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                    d4.gameObject.GetComponentInChildren<Image>().sprite = armouredStegbeetleImage;
                    d4.OnClosed = delegate (DialogResult dr)
                    {
                        Dialog d5 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Dragonflies", "These beautiful creatures can fly straight over your obstacles and will deal a large amount of damage to your Home Tree, so be sure to swat them when you see them.", true);
                        d5.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                        d5.gameObject.GetComponentInChildren<Image>().sprite = dragonflyImage;
                        d5.OnClosed = delegate (DialogResult dr)
                        {
                            Dialog d6 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Hornets", "I'd stay away from these if I were you - swatting will just result in a painful sting - instead, use your plants!", true);
                            d6.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                            d6.gameObject.GetComponentInChildren<Image>().sprite = hornetImage;
                            d6.OnClosed = delegate (DialogResult dr)
                            {
                                _gameStateManager.ContinueTutorialBattle();
                            };
                        };
                        t = (d5.transform.position, d5.transform.rotation);

                    };
                    t = (d4.transform.position, d4.transform.rotation);

                };
                t = (d3.transform.position, d3.transform.rotation);

            };
            t = (d2.transform.position, d2.transform.rotation);

        };
        t = (d1.transform.position, d1.transform.rotation);
    }

    /// <summary>
    /// Displays a button pop-up denoting the end of the tutorial and subsequently commencing the initial round's planning phase.
    /// </summary>
    public void EndTutorial()
    {
        CloseOpenDialogs();
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Tutorial Complete", "Now it's time to start for real. Place your items, and click the red button when you are ready to begin the first battle!", true);
        d1.OnClosed = delegate (DialogResult dr)
        {
            _gameStateManager.BeginRound();
        };
    }

    /// <summary>
    /// Displays a button pop-up denoting the end of the game and the player's score, continuing to the main menu on closure.
    /// </summary>
    public void EndPopUp()
    {
        CloseOpenDialogs();
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Congratulations!", "Score: " + _enemyManager.getEnemiesKilled(), true);
        d.OnClosed += delegate (DialogResult dr)
        {
            GetComponent<StartMenuLogic>().OpenStartMenu();

        };
        lock (_openDialogs)
        {
            _openDialogs.Add(d);
        }
    }

    /// <summary>
    /// Closes any open dialogs in the <c>_openDialogs</c> list.
    /// </summary>
    private void CloseOpenDialogs()
    {
        lock (_openDialogs)
        {
            for (int i = 0; i < _openDialogs.Count; i++)
            {
                _openDialogs[i].DismissDialog();
            }
            _openDialogs.Clear();
        }
    }

    /// <summary>
    /// Resets the local timer to the specified time in seconds.
    /// </summary>
    /// <param name="time">Timer limit in seconds.</param>
    public void ResetTimer(int time)
    {
        _roundTime = time-1;
    }

    /// <summary>
    /// Decreases the timer by 1 second.
    /// </summary>
    public void DecreaseTime()
    {
        _roundTime -= 1;
        infoText.text = "" + _roundTime.ToString();
    }

    bool hypothesisMode = false;
    public void StartShowingHypothesis() { hypothesisMode = true; }
    public void ShowDictation(string dictation)
    {
        hypothesisMode = false;
        dictationText.text = dictation;
        dictationText.color = Color.white;
        StartCoroutine(clearTextAfterDelay(3, dictationText));
    }

    public void ShowMessageAsDictation(string txt)
    {
        dictationText.text = txt;
    }

    public void ShowHypothesis(string hypothesis)
    {
        if (hypothesisMode)
        {
            dictationText.text = hypothesis;
            dictationText.color = Color.red;
        }
    }

    IEnumerator clearTextAfterDelay(int delay,TMP_Text text)
    {
        yield return new WaitForSeconds(delay);
        text.text = "";
    }
}

