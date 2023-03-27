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
    UIController uiController;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        uiController = GetComponent<UIController>();
    }

    float dictationCooldown = 0f;
    private void Update()
    {
        if (dictationCooldown > 0f)
        {
            dictationCooldown -= Time.deltaTime;
        }
    }

    public void Record()
    {
        try
        {
            DictationHandler handler = GetComponent<DictationHandler>();
            lock(handler){
                handler.StartRecording();
                GameObject.FindWithTag("Buddy").GetComponent<Renderer>().material.color = Color.red;
            }
        }catch (System.Exception e)
        {
            txt.text = e.Message;
        }
    }

    public void LightningBolt()
    {
        ProcessDictation("move and attack");
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
        if (dictationCooldown <= 0f)
        {
            uiController.ShowDictation("Initial dictation: " + dictation + "\n");
            string[] words = dictation.Split(' ');
            if (words.Length > 0) StartCoroutine(new LanguageParser(Resources.Load<TextAsset>("basewords").text).GetInstructionStream(words));
            dictationCooldown = 2f;
        }
    }

    public void HandleDictationProcessingResults(List<BuddyAction> instructions)
    {
        string actionStream = "";
        foreach(BuddyAction i in instructions)
        {
            actionStream = actionStream + i.actionType.ToString() + " ";
        }
        GameObject.FindWithTag("Buddy").GetComponent<Renderer>().material.color = Color.white;
        //if(wordData.Length > 0) GetComponent<UIController>().ShowDictation(wordData[0]);
        Debug.Log("Action stream: " + actionStream);
        uiController.ShowDictation("Action stream: "+actionStream);
    }

    IEnumerator Indicator()
    {
        Color defaultColour = pointer.GetComponent<Renderer>().material.color;
        pointer.GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        pointer.GetComponent<Renderer>().material.color = defaultColour;
    }
}
