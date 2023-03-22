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

    public IEnumerator GetSynonyms(string word)
    {
        UnityWebRequest request = UnityWebRequest.Get(getUrl(word));
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.Success)
        {
            GameObject.FindWithTag("Logic").GetComponent<VoiceCommandReceiver>().HandleDictationProcessingResults(
                JArray.Parse(request.downloadHandler.text)[0]["meta"]["syns"][0].ToObject<string[]>()
                );
        }
        else
        {
            Debug.Log("Thesaurus error");
        }
    }

    string getUrl(string word)
    {
        return baseUrl + word + "?key=" + apiKey;
    }
}
