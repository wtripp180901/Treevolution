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
        lock (_openDialogs)
        {
            CloseOpenDialogs(true);
            if (_gameStateManager.currentGameState != GameStateManager.GameState.Calibration_Success)
            {
                Dialog d = Dialog.Open(infoDialogPrefab, DialogButtonType.None, "Calibrate Game Board", "Find and look at the QR Code in the corner of the table to calibrate the Game Board.", true);
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
        lock (_openDialogs)
        {
            CloseOpenDialogs(true);
            Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.Confirm, "Calibration Success", "You have successfully calibrated the Game Board! Click Confirm to lock the board in place.", true);
            d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateGreen;
            d.OnClosed = delegate (DialogResult dr)
            {
                GetComponent<QRDetection>().lockPlane = true;
                GameObject.FindGameObjectWithTag("Floor").GetComponent<Grass>().GenerateGrass(); // Draw the grass on the mapped plane
                TutorialSelectionPopUp();
            };
            _openDialogs.Add(d);
        }
    }

    /// <summary>
    /// Displays an orange info pop-up if the user didn't kill any bugs.
    /// </summary>
    public void NoKillPopUp()
    {
        lock (_openDialogs)
        {
            Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, ":(", "HEY! You didn't play by the rules... I guess you'll just have to continue without any practice.", true);
            d.gameObject.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = backPlateOrange;
            d.OnClosed = delegate (DialogResult dr)
            {
                TutorialBugPopUps();
            };
        }
    }
    /// <summary>
    /// Displays a button pop-up asking the user to select whether they would like to start the Tutorial.
    /// </summary>
    private void TutorialSelectionPopUp()
    {
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.Yes | DialogButtonType.No, "Begin Tutorial", "Would you like to proceed with the tutorial level?", true);
        d.OnClosed += HandleTutorialSelectionEvent;
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
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Treevolution Tutorial", "Protect the Tree from enemy bugs. Either use your hands to damage the bugs, or get help from your plants and ladybird buddy.", true);
        (Vector3 position, Quaternion rotation) t = (d1.transform.position, d1.transform.rotation);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Planning Phase", "There will be four rounds, each beginning with a Planning Phase. During this time, place your obstacles and plants to optimise your strategy; learn from past mistakes.", true);
            d2.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Plants", "You have a range of plants to help defend the Tree, each with different abilities. They show their attack ranges during the Planning Phase to help with your defence.", true);
                d3.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                d3.OnClosed = delegate (DialogResult dr)
                {
                    Dialog d4 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Obstacles", "You also have some long, thorny bushes, which can be used to direct bugs towards your plants and away from the Tree.", true);
                    d4.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                    d4.OnClosed = delegate (DialogResult dr)
                    {
                        Dialog d5 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Give it a Try", "Have a go, place your items where you would like, and when you are happy, press the big red button above the Tree in the centre.\nBring On The Bugs!", true);
                        d5.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                        d5.OnClosed = delegate (DialogResult dr)
                        {
                            _gameStateManager.BeginTutorialPlan();
                        };

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
    /// Displays a number of sequential tutorial pop-ups containing information about the battle phase, the last of which initiates the tutorial battle phase on closure.
    /// </summary>
    public void TutorialBattlePopUps()
    {
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Battle Phase", "During battle, the time remaining is displayed above the Tree. Kill as many bugs as you can before the time runs out!", true);
        (Vector3 position, Quaternion rotation) t = (d1.transform.position, d1.transform.rotation);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Moving Items", "If you are not happy with your items' placements during battle, you can still move them. However, they will become disabled for a short time as a penalty.", true);
            d2.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Bugs", "Keep an eye out for new bugs approaching - they will come from random locations at either far end of the table. The first ones you encounter will be being the Ants.", true);
                d3.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                d3.OnClosed = delegate (DialogResult dr)
                {
                    Dialog d4 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Ants", "Ants are the weakest of the bugs. They have no armour, and can be squashed rather easily.", true);
                    d4.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                    d4.gameObject.GetComponentInChildren<Image>().sprite = antImage;
                    d4.OnClosed = delegate (DialogResult dr)
                    {
                        Dialog d5 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Damage", "Most bugs can be damaged by *whacking* them. This works best by hitting the table just in front of the bug with your hand flat, or in a fist.\nPress OK and try for yourself!", true);
                        d5.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                        d5.gameObject.GetComponentInChildren<Image>().sprite = damageGif;
                        d5.OnClosed = delegate (DialogResult dr)
                        {
                            StartCoroutine(_gameStateManager.BeginTutorialBattle());
                        };

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
    /// Displays a number of sequential tutorial pop-ups containing information about the bug types, the last of which continues the tutorial battle phase on closure.
    /// </summary>
    public void TutorialBugPopUps()
    {
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Bugs", "More types of bugs will be introduced as you progress through the rounds. Some have strong armour, so it may make more sense to target these with your plants or buddy.", true);
        (Vector3 position, Quaternion rotation) t = (d1.transform.position, d1.transform.rotation);
        d1.OnClosed = delegate (DialogResult dr)
        {
            Dialog d2 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Beetles", "The Beetle is stronger than the ant, but its armour slows it down. It takes a few strikes to kill by hand.", true);
            d2.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
            d2.gameObject.GetComponentInChildren<Image>().sprite = armouredBeetleImage;
            d2.OnClosed = delegate (DialogResult dr)
            {
                Dialog d3 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Cockroaches", "The Cockroach is both strong and fast. Its armour may be difficult to break though with brute force alone.", true);
                d3.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                d3.gameObject.GetComponentInChildren<Image>().sprite = armouredCockroachImage;
                d3.OnClosed = delegate (DialogResult dr)
                {
                    Dialog d4 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Stagbeetles", "The Stagbeetle is slow, but it has health and strength to make up for it. It can break down obstacles, and is protected against whacking", true);
                    d4.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                    d4.gameObject.GetComponentInChildren<Image>().sprite = armouredStegbeetleImage;
                    d4.OnClosed = delegate (DialogResult dr)
                    {
                        Dialog d5 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Dragonflies", "These beautiful creatures can fly straight over your obstacles, so be sure to swat them when you see them, or place your fly-trap wisely!", true);
                        d5.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                        d5.gameObject.GetComponentInChildren<Image>().sprite = dragonflyImage;
                        d5.OnClosed = delegate (DialogResult dr)
                        {
                            Dialog d6 = Dialog.Open(buttonImageDiaglogPrefab, DialogButtonType.OK, "Hornets", "Only the most advanced of players can kill these - swatting will just result in a painful sting (to you, not the Hornet).", true);
                            d6.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                            d6.gameObject.GetComponentInChildren<Image>().sprite = hornetImage;
                            d6.OnClosed = delegate (DialogResult dr)
                            {
                                Dialog d7 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Buddy", "As well as whacking bugs and using your plants, you can also use your ladybird buddy by either saying \"Attack\", or \"Defend Here\" whilst pointing to an area.\nPress OK and try for youself!", true);
                                d7.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                                d7.OnClosed = delegate (DialogResult dr)
                                {
                                    _gameStateManager.ContinueTutorialBattle();
                                };
                            };
                            t = (d6.transform.position, d6.transform.rotation);
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
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Tutorial Complete", "Now it's time to start for real. Place your items carefully, and click the red button when you are ready to begin the first battle!", true);
        d1.OnClosed = delegate (DialogResult dr)
        {
            _gameStateManager.BeginRound();
        };
    }

    public void BuddyPopUp()
    {
        Dialog d1 = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Buddy Commands", "Remember to give commands to your buddy, such as \"Attack\", or \"Defend Here\" whilst pointing at a location. If Stagbeetles destroy your obstacles, say \"Repair\" to fix them.", true);
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
        Dialog d = Dialog.Open(buttonDialogPrefab, DialogButtonType.OK, "Congratulations!", "Score: " + _enemyManager.getScore(), true);
        d.OnClosed += delegate (DialogResult dr)
        {
            GetComponent<StartMenuLogic>().OpenStartMenu();
        };
    }

    /// <summary>
    /// Closes any open dialogs in the <c>_openDialogs</c> list.
    /// </summary>
    private void CloseOpenDialogs(bool alreadyLocked)
    {
        void clearOpenDialogs()
        {
            for (int i = 0; i < _openDialogs.Count; i++)
            {
                if (_openDialogs != null)
                {
                    _openDialogs[i].State = DialogState.Closing;
                    _openDialogs[i].DismissDialog();
                }
            }
            _openDialogs.Clear();
        }
        if (alreadyLocked)
        {
            clearOpenDialogs();
        }
        else
        {
            lock (_openDialogs)
            {
                clearOpenDialogs();
            }
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
    /// <summary>
    /// Indicates that the dictationText should currently be displaying the DictationHypothesis from the DictationHandler
    /// </summary>
    public void StartShowingHypothesis() { hypothesisMode = true; }
    /// <summary>
    /// Briefly displays given text on the HUD. Intended for use to show DictationResult's from DictationHandler
    /// </summary>
    /// <param name="dictation">Text to display</param>
    public void ShowDictation(string dictation)
    {
        float origSize = dictationText.fontSize;
        dictationText.fontSize = origSize / 2;
        hypothesisMode = false;
        dictationText.text = dictation;
        dictationText.color = Color.white;
        StartCoroutine(clearTextAfterDelay(30, dictationText, origSize));
    }

    /// <summary>
    /// Shows text on the dictationText HUD but doesn't clear it
    /// </summary>
    /// <param name="txt">Text to be shown</param>
    public void ShowMessageAsDictation(string txt)
    {
        dictationText.text = txt;
    }

    /// <summary>
    /// Displays the text on the dictationText HUD in hypothesis mode
    /// </summary>
    /// <param name="hypothesis">Text to display</param>
    public void ShowHypothesis(string hypothesis)
    {
        if (hypothesisMode)
        {
            dictationText.text = hypothesis;
            dictationText.color = Color.red;
        }
    }

    /// <summary>
    /// Clears a TextMeshPro after a delay
    /// </summary>
    /// <param name="delay">The time the text should stay visible</param>
    /// <param name="text">The TextMeshPro the text is displayed on</param>
	/// <param name="origSize">The original fontsize of the text to be restored to</param>
    /// <returns></returns>
    IEnumerator clearTextAfterDelay(int delay,TMP_Text text, float origSize)
    {
        yield return new WaitForSeconds(delay);
        text.text = "";
        text.fontSize = origSize;
    }
}

