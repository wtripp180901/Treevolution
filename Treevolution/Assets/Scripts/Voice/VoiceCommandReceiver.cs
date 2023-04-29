using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Newtonsoft.Json.Linq;

public class VoiceCommandReceiver : MonoBehaviour
{
    public GameObject pointer;
    private EnemyManager enemyManager;
    PointerLocationTracker pointerTracker;
    UIController uiController;
    GameObject recordingIndicator;
    GameStateManager gameStateManager;

    public bool SafeMode = false;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        uiController = GetComponent<UIController>();
        pointerTracker = GetComponent<PointerLocationTracker>();
        gameStateManager = GetComponent<GameStateManager>();
        recordingIndicator = GameObject.FindWithTag("RecordingIndicator");
        recordingIndicator.SetActive(false);
    }

    private void Update()
    {
        if (currentlyRecording)
        {
            timeRecording += Time.deltaTime;
            if(timeRecording > recordingTimeout)
            {
                finishDictation();
            }
        }
    }

    public void PauseGame()
    {
        if (gameStateManager.currentGameState == GameStateManager.GameState.Round_Battle || gameStateManager.currentGameState == GameStateManager.GameState.Tutorial_Battle)
            GetComponent<RoundTimer>().PauseTimer();
    }

    bool currentlyRecording = false;
    float timeRecording = 0;
    float recordingTimeout = 10;

    bool canManuallyRecord = true;
    public void SetSafeToRecord() { canManuallyRecord = true; }

    public void Record()
    {
        if (SafeMode)
        {
            if (canManuallyRecord)
            {
                recordingIndicator.SetActive(true);
                SafeModeRecordAudio();
                canManuallyRecord = false;
            }
        }
        else
        {
            recordingIndicator.SetActive(true);
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

    public AudioSource recordingAudioSource;
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
            StartCoroutine(GetComponent<AssemblyAPICaller>().UploadFile());
        }catch(Exception e)
        {
            uiController.ShowDictation("File writing error: "+e.Message);
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

    public void LightningBolt()
    {
        pointerTracker.StartSampling();
        pointerTracker.FinishSampling();
        ProcessDictation("Attack ants.");
        /*GameObject[] enemies = enemyManager.enemies;
        Vector2 pointerPoint = new Vector2(pointer.transform.position.x, pointer.transform.position.z);
        for (int i = 0;i < enemies.Length; i++)
        {
            Vector2 enemyPoint = new Vector2(enemies[i].transform.position.x, enemies[i].transform.position.z);

            if ((enemyPoint - pointerPoint).magnitude < 0.25f)
            {
                enemies[i].GetComponent<EnemyScript>().Damage(5);
            }
        }
        GetComponent<AudioSource>().Play();
        StartCoroutine(Indicator());*/
    }

    public void ProcessDictation(string dictation)
    {
        //uiController.ShowDictation("Dictation recieved: " + dictation);
        try
        {
            finishDictation();
            uiController.ShowDictation(dictation);
            string[] words = dictation.Split(' ');
            if (words.Length > 0) StartCoroutine(new LanguageParsing.LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(words,HandleDictationProcessingResults));
        }catch(Exception e)
        {
            uiController.ShowDictation(e.Message);
        }
    }

    public void DictationError(string error)
    {
        uiController.ShowDictation("Dictation error: " + error);
        finishDictation();
    }

    void finishDictation()
    {
        DictationHandler handler = GetComponent<DictationHandler>();
        lock (handler)
        {
            handler.StopRecording();
        }
        pointerTracker.FinishSampling();
        recordingIndicator.SetActive(false);
        currentlyRecording = false;
        timeRecording = 0;
    }

    public void HandleDictationProcessingResults(List<BuddyAction> instructions)
    {
        string actionStream = "";
        foreach(BuddyAction i in instructions)
        {
            actionStream = actionStream + i.actionType.ToString() + " ";
        }
        //if(wordData.Length > 0) GetComponent<UIController>().ShowDictation(wordData[0]);
        Debug.Log("Action stream: " + actionStream);
        GameObject.FindWithTag("Buddy").GetComponent<BuddyScript>().GiveInstructions(instructions);
    }

    IEnumerator Indicator()
    {
        Color defaultColour = pointer.GetComponent<Renderer>().material.color;
        pointer.GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        pointer.GetComponent<Renderer>().material.color = defaultColour;
    }
}
