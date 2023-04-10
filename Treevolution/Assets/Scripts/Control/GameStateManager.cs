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
    public GameObject debugObject;
    /// <summary>
    /// Returns the current GameState of the game.
    /// </summary>
    public GameState currentGameState { get { return _currentState; } }
    /// <summary>
    /// Specifies the maximum number of rounds to play.
    /// </summary>
    public int maxRoundNumber = 4;
    /// <summary>
    /// Running QRDetection instance.
    /// </summary>
    private QRDetection _qRDetection;
    /// <summary>
    /// Running UIController instance.
    /// </summary>
    private UIController _uIController;
    /// <summary>
    /// Running EnemyManager instance.
    /// </summary>
    private EnemyManager _enemyManager;
    /// <summary>
    /// Running RoundTimer instance.
    /// </summary>
    private RoundTimer _roundTimer;
    /// <summary>
    /// The current GameState value.
    /// </summary>
    private GameState _currentState = GameState.Calibration;
    /// <summary>
    /// The current round number (0 = None/Tutorial, 1 = Round 1, etc.).
    /// </summary>
    private int _currentRoundNumber = 0;

    /// <summary>
    /// Values the GameState can take.
    /// </summary>
    public enum GameState
    {
        Calibration,
        Calibration_Success,
        Tutorial_Plan,
        Tutorial_Battle,
        Round_Plan,
        Round_Battle
    }
    /// <summary>
    /// Types of enemies.
    /// </summary>
    public enum EnemyType
    {
        Ant,
        Armoured_Bug,
        Armoured_Cockroach,
        Armoured_Stagbeetle,
        Dragonfly,
        Hornet
    }
    /// <summary>
    /// Count of each EnemyType to spawn during each round.
    /// </summary>
    private Dictionary<EnemyType, int>[] _enemyWaves = new Dictionary<EnemyType, int>[]{
            new Dictionary<EnemyType, int>(){
                { EnemyType.Ant, 10 },
            },
            new Dictionary<EnemyType, int>(){
                { EnemyType.Ant, 10 },
                { EnemyType.Armoured_Bug, 10 }
            },
            new Dictionary<EnemyType, int>(){
                { EnemyType.Ant, 10 },
                { EnemyType.Armoured_Bug, 8 },
                { EnemyType.Armoured_Cockroach, 8}
            },
            new Dictionary<EnemyType, int>(){
                { EnemyType.Ant, 8 },
                { EnemyType.Armoured_Bug, 8 },
                { EnemyType.Armoured_Cockroach, 8},
                { EnemyType.Armoured_Stagbeetle, 5 },
                { EnemyType.Dragonfly, 5},

            },
            new Dictionary<EnemyType, int>(){
                { EnemyType.Ant, 8 },
                { EnemyType.Armoured_Bug, 5 },
                { EnemyType.Armoured_Cockroach, 8},
                { EnemyType.Armoured_Stagbeetle, 5 },
                { EnemyType.Dragonfly, 5},
                { EnemyType.Hornet, 5}
            }
        };

    

    public void SetupGameStateManagerTesting()
    {
        BeginBattleButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _uIController = gameObject.AddComponent<UIController>();
        _qRDetection = gameObject.AddComponent<QRDetection>();
        _roundTimer = gameObject.AddComponent<RoundTimer>();
        _enemyManager = gameObject.AddComponent<EnemyManager>();
        gameObject.AddComponent<PlaneMapper>();
        _uIController.SetupForTest();
        _enemyManager.SetupForTest();
    }

    /// <summary>
    /// Start runs when loading the GameObject that this script is attached to.
    /// </summary>
    private void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            debugObject?.SetActive(true); // Unity Editor Mode
        }
        if(InfoText != null)
            InfoText.text = "";
        _currentState = GameState.Calibration;
        _uIController = GetComponent<UIController>();
        _qRDetection = GetComponent<QRDetection>();
        _enemyManager = GetComponent<EnemyManager>();
        _roundTimer = GetComponent<RoundTimer>();
        _uIController.CalibrationPopUp();
    }

    /// <summary>
    /// Called when the GameBoard QR Code is first recognised to move the state to <c>Plane_Mapped</c> and open a success dialog.
    /// </summary>
    public void CalibrationSuccess()
    {
        _currentState = GameState.Calibration_Success;
        _uIController.CalibrationSuccessPopUp();
    }

    /// <summary>
    /// Starts the Tutorial if it is selected.
    /// </summary>
    public void BeginTutorial()
    {
        _currentRoundNumber = 0;
        _uIController.TutorialPlanPopUps();
    }

    /// <summary>
    /// Begins the Tutorial Planning Phase and progresses the state to <c>Tutorial_Plan</c>.
    /// </summary>
    public void BeginTutorialPlan()
    {
        _currentState = GameState.Tutorial_Plan;
        if (InfoText != null)
        {
             InfoText.transform.position = GameProperties.Centre + new Vector3(0, 0.65f, 0);
             InfoText.text = "Tutorial\n[Planning]";
        }
        BeginBattleButton.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        BeginBattleButton.SetActive(true);
    }

    /// <summary>
    /// Begins the Tutorial Battle Information pop-ups being displayed.
    /// </summary>
    /// <returns>This method runs a coroutine and so a <c>yield return</c> is used.</returns>
    private IEnumerator BeginTutorialBattleInfo()
    {
        _roundTimer.SetRoundLength(30);
        _enemyManager.StartSpawning(_enemyWaves[0]);
        _roundTimer.StartTimer(); // play
        yield return new WaitForSeconds(1);
        _roundTimer.PauseTimer();
        _uIController.TutorialBattlePopUps();
    }

    /// <summary>
    /// Begins the Tutorial Battle phase.
    /// </summary>
    /// <returns>This method runs a coroutine and so a <c>yield return</c> is used.</returns>
    public IEnumerator BeginTutorialBattle()
    {
        _roundTimer.PauseTimer(); // play
        yield return new WaitUntil(() => _enemyManager.getEnemiesKilled() != 0);
        _roundTimer.PauseTimer(); // pause
        _uIController.TutorialBugPopUps();
    }

    /// <summary>
    /// Continues the Tutorial Battle phase after the pop-ups have been read.
    /// </summary>
    public void ContinueTutorialBattle()
    {
        _roundTimer.PauseTimer();
    }

    /// <summary>
    /// Ends the Tutorial Battle phase and displays the enemies killed.
    /// </summary>
    /// <param name="enemiesKilled">Number of enemies killed.</param>
    /// <returns>This method runs a coroutine and so a <c>yield return</c> is used.</returns>
    private IEnumerator EndTutorialBattle(int enemiesKilled)
    {
        _enemyManager.resetEnemiesKilled();
         if(InfoText != null) 
            InfoText.text = "Tutorial Over\n[" + enemiesKilled.ToString() + " Enemies Killed]";
        yield return new WaitForSeconds(3);
        _uIController.EndTutorial();
    }

    /// <summary>
    /// Begins a regular game round and progresses the state to <c>Round_Plan</c>.
    /// </summary>
    public void BeginRound()
    {
        _currentState = GameState.Round_Plan;
        _roundTimer.SetRoundLength(60);
        _currentRoundNumber++;
        if (InfoText != null)
        {
            InfoText.transform.position = GameProperties.Centre + new Vector3(0, 0.65f, 0);
            InfoText.text = "Round " + _currentRoundNumber.ToString() + "\n[Planning]";
        }
        BeginBattleButton.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        BeginBattleButton.SetActive(true);
    }

    /// <summary>
    /// Run by a button press, and begins the enemy spawning for the current round. If it isn't a tutorial it progresses the state to <c>Round_Battle</c>, otherwise to <c>Tutorial_Battle</c>.
    /// </summary>
    public void BeginBattle()
    {
        BeginBattleButton.SetActive(false);
        if(InfoText != null)
             InfoText.transform.position = GameProperties.Centre + new Vector3(0, 0.5f, 0);
        //_qRDetection.StopQR();
        GameProperties.BattlePhase = true;
        if (_currentState == GameState.Tutorial_Plan)
        {
            _currentState = GameState.Tutorial_Battle;
            StartCoroutine(BeginTutorialBattleInfo());
            return;
        }
        else
        {
            _currentState = GameState.Round_Battle;
            _enemyManager.StartSpawning(_enemyWaves[_currentRoundNumber]);
            _roundTimer.StartTimer();
        }
    }

    /// <summary>
    /// Ends the battle phase and prepares for the next planning phase or alternatively ends the game if has completed the final round.
    /// </summary>
    /// <returns>This method runs a coroutine and so a <c>yield return</c> is used.</returns>
    public IEnumerator EndBattle()
    {
        GameProperties.BattlePhase = false;
        clearEnemies();
        repairAllWalls();
        _enemyManager.StopSpawning();
        //_qRDetection.StartQR();
        int enemiesKilled = GetComponent<EnemyManager>().getEnemiesKilled();
        if (currentGameState == GameState.Tutorial_Battle)
        {
            StartCoroutine(EndTutorialBattle(enemiesKilled));
            yield break;
        }
        if(InfoText != null)
            InfoText.text = "Round " + _currentRoundNumber.ToString() + " Over\n[" + enemiesKilled.ToString() + " Enemies Killed]";
        yield return new WaitForSeconds(3);
        if (_currentRoundNumber < maxRoundNumber)
        {
            BeginRound();
        }
        else
        {
            EndGame();
        }
    }

    /// <summary>
    /// Ends the game and calls a pop-up which displays the player's score.
    /// </summary>
    private void EndGame()
    {
        _uIController.EndPopUp();
    }

    /// <summary>
    /// Destroys all enemies still active the EnemyManager.
    /// </summary>
    private void clearEnemies()
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyList.Length; i++)
        {
            enemyList[i].GetComponent<EnemyScript>().DestroyEnemy(false);
        }
    }

    private void repairAllWalls()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        for(int i = 0;i < walls.Length; i++)
        {
            walls[i].GetComponent<WallScript>().Repair(10);
        }
    }
}
