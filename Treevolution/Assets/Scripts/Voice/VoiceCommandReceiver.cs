using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class VoiceCommandReceiver : MonoBehaviour
{
    public GameObject pointer;
    private EnemyManager enemyManager;
    public TMP_Text txt;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
    }

    public void Record()
    {
        try
        {
            DictationHandler handler = GetComponent<DictationHandler>();
            lock(handler){
                handler.StartRecording();
            }
        }catch (System.Exception e)
        {
            txt.text = e.Message;
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
        string[] words = dictation.Split(' ');
        if(words.Length > 0) StartCoroutine(new ThesaurusAPICaller("APIKey").GetSynonyms(words[0]));
    }

    public void HandleDictationProcessingResults(string[] synonyms)
    {
        if(synonyms.Length > 0) GetComponent<UIController>().ShowDictation(synonyms[0]);
    }

    IEnumerator Indicator()
    {
        Color defaultColour = pointer.GetComponent<Renderer>().material.color;
        pointer.GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        pointer.GetComponent<Renderer>().material.color = defaultColour;
    }
}
