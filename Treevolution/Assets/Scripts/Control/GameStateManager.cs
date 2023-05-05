using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    /// The default round length, obtained from the round timer.
    /// </summary>
    private int _defaultRoundLength;
    /// <summary>
    /// Audio Source to play the background music from.
    /// </summary>
    [SerializeField]
    private AudioSource _musicPlayer;
    /// <summary>
    /// Planning Phase Music
    /// </summary>
    [SerializeField]
    private AudioClip _planningMusic;
    /// <summary>
    /// Indicates if the planning phase music is playing or not.
    /// </summary>
    private bool _planningMusicPlaying = true;
    /// <summary>
    /// Battle Phase Music Loop
    /// </summary>
    [SerializeField]
    private AudioClip _battleMusicLoop;
    /// <summary>
    /// Indicates if the Battle Phase Music Loop is playing or not.
    /// </summary>
    private bool _battleMusicPlaying = false;
    /// <summary>
    /// Battle Phase Music 1
    /// </summary>
    [SerializeField]
    private AudioClip _battleMusic1;
    /// <summary>
    /// Battle Phase Music 2
    /// </summary>
    [SerializeField]
    private AudioClip _battleMusic2;
    /// <summary>
    /// Battle Phase Music 3
    /// </summary>
    [SerializeField]
    private AudioClip _battleMusic3;
    /// <summary>
    /// Battle Phase Music 4
    /// </summary>
    [SerializeField]
    private AudioClip _battleMusic4;
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
    /// Running TowerManager instance.
    /// </summary>
    private TowerManager _towerManager;
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
        Round_Battle,
        Start_Menu
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
                { EnemyType.Armoured_Cockroach, 8},
                { EnemyType.Armoured_Stagbeetle, 5 }
            },
            new Dictionary<EnemyType, int>(){
                { EnemyType.Ant, 8 },
                { EnemyType.Armoured_Bug, 8 },
                { EnemyType.Armoured_Cockroach, 8},
                { EnemyType.Armoured_Stagbeetle, 5},
                { EnemyType.Dragonfly, 5},

            },
            new Dictionary<EnemyType, int>(){
                { EnemyType.Ant, 8 },
                { EnemyType.Armoured_Bug, 5 },
                { EnemyType.Armoured_Cockroach, 8},
                { EnemyType.Armoured_Stagbeetle, 5},
                { EnemyType.Dragonfly, 5},
                { EnemyType.Hornet, 5}
            }
        };

    /// <summary>
    /// Sets up required class variables for a testing environment.
    /// </summary>
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
    /// Toggles the currently playing music track depending on the round number and phase (planning or battle).
    /// </summary>
    public void ToggleMusic()
    {
        if (_planningMusicPlaying)
        {
            _musicPlayer.Stop();
            _planningMusicPlaying = false;
            AudioClip battleClip = _battleMusicLoop;
            _musicPlayer.loop = false;
            _musicPlayer.time = 0f;
            switch (_currentRoundNumber)
            {
                case 0:
                    _musicPlayer.time = 1.8f;
                    _musicPlayer.loop = true;
                    break;
                case 1:
                    battleClip = _battleMusic1;
                    break;
                case 2:
                    battleClip = _battleMusic2;
                    break;
                case 3:
                    battleClip = _battleMusic3;
                    break;
                case 4:
                    battleClip = _battleMusic4;
                    break;
            }

            _musicPlayer.clip = battleClip;
            _musicPlayer.Play();
            _battleMusicPlaying = true;
        }
        else
        {
            _musicPlayer.Stop();
            _battleMusicPlaying = false;
            _musicPlayer.clip = _planningMusic;
            _musicPlayer.loop = true;
            _musicPlayer.Play();
            _planningMusicPlaying = true;

        }
    }

    /// <summary>
    /// Initialises variables of the GameStateManager for starting the game.
    /// </summary>
    public void InitGameState()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            debugObject?.SetActive(true); // Unity Editor Mode
        }
        if (InfoText != null)
        {
            InfoText.text = "";
            InfoText.gameObject.SetActive(true);
        }
        _currentState = GameState.Calibration;
        _currentRoundNumber = 0;
        _uIController = GetComponent<UIController>();
        _qRDetection = GetComponent<QRDetection>();
        _enemyManager = GetComponent<EnemyManager>();
        _enemyManager.resetEnemiesKilled();
        _roundTimer = GetComponent<RoundTimer>();
        _defaultRoundLength = _roundTimer.GetRoundLength();
        _towerManager = GetComponent<TowerManager>();
        _qRDetection.StartQR();
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
        BeginBattleButton.GetComponent<PressableButton>().enabled = false;
        BeginBattleButton.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        BeginBattleButton.GetComponent<PressableButton>().enabled = true;
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
        _enemyManager.toggleDamage(false);
        _towerManager.DisableAllTowers(true);
        _roundTimer.PauseTimer(true);
        _uIController.TutorialBattlePopUps();
    }

    /// <summary>
    /// Begins the Tutorial Battle phase.
    /// </summary>
    /// <returns>This method runs a coroutine and so a <c>yield return</c> is used.</returns>
    public IEnumerator BeginTutorialBattle()
    {
        _enemyManager.toggleDamage(true);
        _roundTimer.PauseTimer(false); // play
        yield return new WaitUntil(() => _enemyManager.getEnemiesKilled() != 0 || _uIController.timeRemaining == 1);
        _enemyManager.toggleDamage(false);
        _roundTimer.PauseTimer(true); // pause
        if (_uIController.timeRemaining <= 1)
        {
            _uIController.NoKillPopUp();
            yield break;
        }
        _uIController.TutorialBugPopUps();
    }

    /// <summary>
    /// Continues the Tutorial Battle phase after the pop-ups have been read.
    /// </summary>
    public void ContinueTutorialBattle()
    {
        _enemyManager.toggleDamage(true);
        _towerManager.DisableAllTowers(false);
        _roundTimer.PauseTimer(false);
    }

    /// <summary>
    /// Ends the Tutorial Battle phase and displays the enemies killed.
    /// </summary>
    /// <param name="enemiesKilled">Number of enemies killed.</param>
    /// <returns>This method runs a coroutine and so a <c>yield return</c> is used.</returns>
    private IEnumerator EndTutorialBattle(int score)
    {
        _enemyManager.resetEnemiesKilled();
         if(InfoText != null) 
            InfoText.text = "Tutorial Over\n[Score: " + score.ToString() + "]";
        yield return new WaitForSeconds(3);
        ToggleMusic();
        _uIController.EndTutorial();
    }

    /// <summary>
    /// Begins a regular game round and progresses the state to <c>Round_Plan</c>.
    /// </summary>
    public void BeginRound()
    {
        _currentState = GameState.Round_Plan;
        _roundTimer.SetRoundLength(_defaultRoundLength);
        _currentRoundNumber++;
        if (InfoText != null)
        {
            InfoText.transform.position = GameProperties.Centre + new Vector3(0, 0.65f, 0);
            InfoText.text = "Round " + _currentRoundNumber.ToString() + "\n[Planning]";
        }
        BeginBattleButton.SetActive(true);
        BeginBattleButton.GetComponent<PressableButton>().enabled = false;
        BeginBattleButton.transform.position = GameProperties.Centre + new Vector3(0, 0.6f, 0);
        BeginBattleButton.GetComponent<PressableButton>().enabled = true;
        _towerManager.ToggleAllRangeVisuals(true);
    }

    /// <summary>
    /// Run by a button press, and begins the enemy spawning for the current round. If it isn't a tutorial it progresses the state to <c>Round_Battle</c>, otherwise to <c>Tutorial_Battle</c>.
    /// </summary>
    public void BeginBattle()
    {
        BeginBattleButton.SetActive(false);
        if (InfoText != null)
             InfoText.transform.position = GameProperties.Centre + new Vector3(0, 0.5f, 0);
        GameProperties.BattlePhase = true;
        ToggleMusic();
        _towerManager.ToggleAllRangeVisuals(false);
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
        int score = GetComponent<EnemyManager>().getScore();
        if (currentGameState == GameState.Tutorial_Battle)
        {
            StartCoroutine(EndTutorialBattle(score));
            yield break;
        }
        if(InfoText != null)
            InfoText.text = "Round " + _currentRoundNumber.ToString() + " Over\n[Score: " + score.ToString() + "]";
        yield return new WaitForSeconds(3);
        ToggleMusic();
        if (_currentRoundNumber + 1 == 2)
        {
            _uIController.BuddyPopUp(); // Introduces Stag beetle mechanics
        }
        else if (_currentRoundNumber < maxRoundNumber)
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
        _currentState = GameState.Start_Menu;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            debugObject.SetActive(false); // Unity Editor Mode
        }
        if (InfoText != null)
        {
            InfoText.text = "";
            InfoText.gameObject.SetActive(false);
        }
        _qRDetection.StopQR();
        _qRDetection.lockPlane = false;
        _qRDetection.ResetTrackedCodes();
        GetComponent<PlaneMapper>().ResetPlane();
        Destroy(GameObject.FindGameObjectWithTag("Tree"));
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

    /// <summary>
    /// Repairs all the walls, currently active in the game.
    /// </summary>
    private void repairAllWalls()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        for(int i = 0;i < walls.Length; i++)
        {
            walls[i].GetComponent<WallScript>().Repair(10);
        }
    }
}
