using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Newtonsoft.Json.Linq;

/// <summary>
/// A controller to receive events from recognised keywords and dictation
/// </summary>
public class VoiceCommandReceiver : MonoBehaviour
{
    /// <summary>
    /// The object representing PointerLocationTracker's pointer
    /// </summary>
    public GameObject pointer;
    private EnemyManager enemyManager;
    PointerLocationTracker pointerTracker;
    UIController uiController;
    GameObject recordingIndicator;
    GameStateManager gameStateManager;

    /// <summary>
    /// The audio that should play to indicate the buddy has begun or finished recording
    /// </summary>
    [SerializeField] AudioSource recordingIndicationSource;
    float basePitch;

    public bool SafeMode = false;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        uiController = GetComponent<UIController>();
        pointerTracker = GetComponent<PointerLocationTracker>();
        gameStateManager = GetComponent<GameStateManager>();
        recordingIndicator = GameObject.FindWithTag("RecordingIndicator");
        recordingIndicator.SetActive(false);
        basePitch = recordingIndicationSource.pitch;
    }

    private void Update()
    {
        if (currentlyRecording)
        {
            timeRecording += Time.deltaTime;
            if (timeRecording > recordingTimeout)
            {
                finishDictation();
            }
        }
    }

    void playStartOfRecordingSound()
    {
        recordingIndicationSource.pitch = basePitch + 0.5f;
        recordingIndicationSource.Play();
    }

    void playEndOfRecordingSound()
    {
        recordingIndicationSource.pitch = basePitch;
        recordingIndicationSource.Play();
    }

    /// <summary>
    /// Toggles pausing the game
    /// </summary>
    public void PauseGame()
    {
        if (gameStateManager.currentGameState == GameStateManager.GameState.Round_Battle || gameStateManager.currentGameState == GameStateManager.GameState.Tutorial_Battle)
            GetComponent<RoundTimer>().PauseTimer();
    }

    bool currentlyRecording = false;
    float timeRecording = 0;
    float recordingTimeout = 10;

    bool canManuallyRecord = true;
    /// <summary>
    /// Indicates that writing to the recording.wav file on the persistentDataPath has finished and it is safe to record and write a new file
    /// </summary>
    public void SetSafeToRecord() { canManuallyRecord = true; }

    /// <summary>
    /// If in safe mode, will begin the recording session to write to a file to be uploaded to AssemblyAI. If not in safe mode, will begin the dictation session for DictationHandler. Both play a recording sound and cause the buddy's indication effect to play
    /// </summary>
    public void Record()
    {
        if (SafeMode)
        {
            if (canManuallyRecord)
            {
                recordingIndicator.SetActive(true);
                playStartOfRecordingSound();
                SafeModeRecordAudio();
                canManuallyRecord = false;
            }
        }
        else
        {
            recordingIndicator.SetActive(true);
            playStartOfRecordingSound();
            try
            {
                uiController.StartShowingHypothesis();
                DictationHandler handler = GetComponent<DictationHandler>();
                pointerTracker.StartSampling();
                lock (handler)
                {
                    handler.StartRecording();
                }
                currentlyRecording = true;
                //StartCoroutine(recordingTimeout());
            }
            catch (System.Exception e)
            {
                uiController.ShowDictation(e.Message);
                Debug.Log(e.Message);
            }
        }
    }

    /// <summary>
    /// The AudioSource recorded voice data should be saved to
    /// </summary>
    public AudioSource recordingAudioSource;
    /// <summary>
    /// Begins the recording session if in safe mode. Will automatically finish session after 4 seconds
    /// </summary>
    void SafeModeRecordAudio()
    {
        try
        {
            uiController.ShowMessageAsDictation("I'm listening...");
            int min, max;
            Microphone.GetDeviceCaps(Microphone.devices[0], out min, out max);
            recordingAudioSource.clip = Microphone.Start(Microphone.devices[0], true, 4, min);
            StartCoroutine(writeAfterFinished());
        }
        catch (Exception e)
        {
            uiController.ShowDictation(e.Message);
            recordingIndicator.SetActive(false);
            canManuallyRecord = true;
        }
    }

    /// <summary>
    /// Writes data from a safeMode recording session to a file on the persistent data path and then begins AssemblyAPICaller's uploaded and processing process
    /// </summary>
    /// <returns></returns>
    IEnumerator writeAfterFinished()
    {
        yield return new WaitForSeconds(4.5f);
        pointerTracker.StartSampling();
        pointerTracker.FinishSampling();
        try
        {
            Microphone.End(Microphone.devices[0]);
            string otp;
            SavWav.Save("recording", recordingAudioSource.clip, out otp);
            recordingIndicator.SetActive(false);
            playEndOfRecordingSound();
            StartCoroutine(GetComponent<AssemblyAPICaller>().UploadFile());
        }
        catch (Exception e)
        {
            uiController.ShowDictation("File writing error: " + e.Message);
            canManuallyRecord = true;
        }
    }

    /*IEnumerator recordingTimeout()
    {
        yield return new WaitForSeconds(10.5f);
        if (currentlyRecording)
        {
            finishDictation();
            GetComponent<UIController>().ShowDictation("Sorry, I can't talk to Microsoft!");
        }
    }*/

    /// <summary>
    /// Should be called when the 'thunder' voice command is received. Damages enemies indicated by the pointer
    /// </summary>
    public void LightningBolt()
    {
        GameObject[] enemies = enemyManager.enemies;
        Vector2 pointerPoint = new Vector2(pointer.transform.position.x, pointer.transform.position.z);
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector2 enemyPoint = new Vector2(enemies[i].transform.position.x, enemies[i].transform.position.z);

            if ((enemyPoint - pointerPoint).magnitude < 0.25f)
            {
                enemies[i].GetComponent<EnemyScript>().Damage(5);
            }
        }
        GetComponent<AudioSource>().Play();
        StartCoroutine(Indicator());
    }

    /// <summary>
    /// Displays the dictation received from a recording session and sends it to the language parsing model to be translated to buddy instructions
    /// </summary>
    /// <param name="dictation"></param>
    public void ProcessDictation(string dictation)
    {
        //uiController.ShowDictation("Dictation recieved: " + dictation);
        try
        {
            finishDictation();
            uiController.ShowDictation(dictation);
            string[] words = dictation.Split(' ');
            if (words.Length > 0) StartCoroutine(new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(words, HandleDictationProcessingResults));
        }
        catch (Exception e)
        {
            uiController.ShowDictation(e.Message);
        }
    }

    /// <summary>
    /// Should be called by the DictationHandler's onDictationErrorEvent
    /// </summary>
    /// <param name="error">Dynamic string for the error message</param>
    public void DictationError(string error)
    {
        uiController.ShowDictation("Dictation error: " + error);
        finishDictation();
    }

    /// <summary>
    /// Ends a dictation recording session and disables all dictation indicators
    /// </summary>
    void finishDictation()
    {
        DictationHandler handler = GetComponent<DictationHandler>();
        lock (handler)
        {
            handler.StopRecording();
        }
        pointerTracker.FinishSampling();
        recordingIndicator.SetActive(false);
        playEndOfRecordingSound();
        currentlyRecording = false;
        timeRecording = 0;
    }

    /// <summary>
    /// A callback function for the language parsing model to return resolved buddy instructions. Sends these instructions to the buddy
    /// </summary>
    /// <param name="instructions"></param>
    public void HandleDictationProcessingResults(List<BuddyAction> instructions)
    {
        string actionStream = "";
        foreach (BuddyAction i in instructions)
        {
            actionStream = actionStream + i.actionType.ToString() + " ";
        }
        //if(wordData.Length > 0) GetComponent<UIController>().ShowDictation(wordData[0]);
        Debug.Log("Action stream: " + actionStream);
        GameObject.FindWithTag("Buddy").GetComponent<BuddyScript>().GiveInstructions(instructions);
    }

    /// <summary>
    /// Makes the pointer briefly flash red
    /// </summary>
    /// <returns></returns>
    IEnumerator Indicator()
    {
        Color defaultColour = pointer.GetComponent<Renderer>().material.color;
        pointer.GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        pointer.GetComponent<Renderer>().material.color = defaultColour;
    }
}
