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
    public TMP_Text txt;
    PointerLocationTracker pointerTracker;
    UIController uiController;
    GameObject recordingIndicator;
    GameStateManager gameStateManager;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        uiController = GetComponent<UIController>();
        pointerTracker = GetComponent<PointerLocationTracker>();
        gameStateManager = GetComponent<GameStateManager>();
        recordingIndicator = GameObject.FindWithTag("RecordingIndicator");
        recordingIndicator.SetActive(false);
    }
	
	public void PauseGame()
    {
        if (gameStateManager.currentGameState == GameStateManager.GameState.Round_Battle || gameStateManager.currentGameState == GameStateManager.GameState.Tutorial_Battle)
            GetComponent<RoundTimer>().PauseTimer();
    }

    public void Record()
    {
        recordingIndicator.SetActive(true);
        try
        {
            DictationHandler handler = GetComponent<DictationHandler>();
            pointerTracker.StartSampling();
            lock(handler){
                handler.StartRecording();
            }
        }catch (System.Exception e)
        {
            txt.text = e.Message;
            Debug.Log(e.Message);
        }
    }

    public void LightningBolt()
    {
        GameObject[] enemies = enemyManager.enemies;
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
        StartCoroutine(Indicator());
    }

    public void ProcessDictation(string dictation)
    {
        DictationHandler handler = GetComponent<DictationHandler>();
        lock (handler)
        {
            handler.StopRecording();
        }
        uiController.ShowDictation(dictation);
        string[] words = dictation.Split(' ');
        if (words.Length > 0) StartCoroutine(new LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(words));
        recordingIndicator.SetActive(false);
    }

    public void HandleDictationProcessingResults(List<BuddyAction> instructions)
    {
        pointerTracker.FinishSampling();
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
