using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;
using Newtonsoft.Json.Linq;

[Serializable]
public class SynonymsObject
{
    public object meta;
    public object hwi;
    public string fl;
    public object[] def;
    public string[] shortdef;
}

public class ThesaurusAPICaller
{
    string baseUrl = "https://www.dictionaryapi.com/api/v3/references/thesaurus/json/";
    string apiKey;
    public ThesaurusAPICaller(string keyFile)
    {
        try
        {
            apiKey = Resources.Load<TextAsset>(keyFile).text;
        }catch(Exception e)
        {
            Debug.Log("If you're seeing this error, ask Will for the API key!");
        }
    }

    public IEnumerator GetSynonyms(string[] words)
    {
        JArray[] wordData = new JArray[words.Length];
        for(int i = 0;i < words.Length; i++) { 
            UnityWebRequest request = UnityWebRequest.Get(getUrl(words[i]));
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                wordData[i] = JArray.Parse(request.downloadHandler.text);
            }
            else
            {
                Debug.Log("Thesaurus error");
            }
        }
        GameObject.FindWithTag("Logic").GetComponent<VoiceCommandReceiver>().HandleDictationProcessingResults(wordData);
        /*if(request.result == UnityWebRequest.Result.Success)
        {
            JArray data = JArray.Parse(request.downloadHandler.text);
            for(int i = 0;i < data.Count; i++)
            {
                if(data[i]["fl"].ToString() == desiredFuncLabel)
                {
                    GameObject.FindWithTag("Logic").GetComponent<VoiceCommandReceiver>().HandleDictationProcessingResults(
                        JArray.Parse(request.downloadHandler.text)[i]["meta"]["syns"][0].ToObject<string[]>()
                    );
                }
            }
            
        }
        else
        {
            Debug.Log("Thesaurus error");
        }*/
    }

    string getUrl(string word)
    {
        return baseUrl + word + "?key=" + apiKey;
    }
}
