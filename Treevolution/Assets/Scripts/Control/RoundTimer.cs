using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the timer for each round and invokes events for each second tick and round over.
/// </summary>
public class RoundTimer : MonoBehaviour
{
    /// <summary>
    /// Length of the round in seconds.
    /// </summary>
    /// 
    public float roundLengthSecs = 60;
    /// <summary>
    /// Pause state of the timer.
    /// </summary>
    public bool isPaused { get { return _isPaused; } }
    /// <summary>
    /// Running state of the timer.
    /// </summary>
    public bool isRunning { get { return _isRunning; } }

    /// <summary>
    /// Private timer pause toggle.
    /// </summary>
    private bool _isPaused = false;
    /// <summary>
    /// Private timer stopped toggle.
    /// </summary>
    private bool _isStopped = false;
    /// <summary>
    /// Private timer running toggle.
    /// </summary>
    private bool _isRunning = false;


    /// <summary>
    /// Local timer which counts up.
    /// </summary>
    private float _roundTimer = 0.0f;
    /// <summary>
    /// Local second timer which counts up to 1 and then resets continuously.
    /// </summary>
    private float _secondTimer = 0.0f;

    /// <summary>
    /// Event called on each second tick whilst the timer is running.
    /// </summary>
    public UnityEvent SecondTickEvent = new UnityEvent();
    /// <summary>
    /// Event called when the timer exceeds the round length.
    /// </summary>
    public UnityEvent RoundOverEvent = new UnityEvent();
    /// <summary>
    /// Event called when the timer is stopped.
    /// </summary>
    public UnityEvent StopTimerEvent = new UnityEvent();
    /// <summary>
    /// Event called when the timer is paused.
    /// </summary>
    public UnityEvent PauseTimerEvent = new UnityEvent();
    /// <summary>
    /// Event called when the timer is started.
    /// </summary>
    public UnityEvent StartTimerEvent = new UnityEvent();

    /// <summary>
    /// Start runs when loading the GameObject that this script is attached to.
    /// </summary>
    void Start()
    {
        RoundOverEvent.AddListener(() => StartCoroutine(GetComponent<GameStateManager>().EndBattle()));
        StartTimerEvent.AddListener(() => GetComponent<UIController>().ResetTimer((int)roundLengthSecs));
        StopTimerEvent.AddListener(() => Debug.Log("Timer Stopped"));
        PauseTimerEvent.AddListener(() => Debug.Log("Pause/Play"));
        PauseTimer();
    }
    
    /// <summary>
    /// Starts the timer and invokes the start timer event.
    /// </summary>
    public void StartTimer()
    {
        _roundTimer = 0;
        _secondTimer = 0;
        _isStopped = false;
        _isPaused = false;
        _isRunning = true;
        StartTimerEvent?.Invoke();
    }

    /// <summary>
    /// Update runs on each frame update, in this case it increases both local timer variables and checks their values, calling any necessary events.
    /// </summary>
    void Update()
    {
        if (_isPaused || _isStopped)
        {
            _secondTimer = 0;
            return;
        }

        _secondTimer += Time.deltaTime;
        _roundTimer += Time.deltaTime;

        if (_secondTimer >= 1f)
        {
            _secondTimer = 0;
            SecondTick();
        }

        if (_roundTimer >= roundLengthSecs)
        {
            StopTimer();
            RoundOver();
        }

    }

    /// <summary>
    /// Stops the timer, invoking the stop timer event.
    /// </summary>
    public void StopTimer()
    {
        _isRunning = false;
        _isStopped = true;
        _isPaused = false;
        StopTimerEvent?.Invoke();
    }
    
    /// <summary>
    /// Toggles the pause state of the timer, invoking the pause timer event.
    /// </summary>
    public void PauseTimer()
    {
        _isPaused = !_isPaused;
        _isRunning = !_isRunning;
        PauseTimerEvent?.Invoke();
    }

    /// <summary>
    /// Sets the round length to the given number of seconds.
    /// </summary>
    /// <param name="secs">Number of seconds to set the round length to.</param>
    public void SetRoundLength(int secs)
    {
        roundLengthSecs = secs;
    }

    /// <summary>
    /// Invokes the second tick event.
    /// </summary>
    public void SecondTick()
    {
        SecondTickEvent?.Invoke();
    }
    /// <summary>
    /// Invokes the round over event.
    /// </summary>
    public void RoundOver()
    {
        RoundOverEvent?.Invoke();
    }
}
