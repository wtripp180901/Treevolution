using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class AssemblyAPICaller : MonoBehaviour
{
    string key;
    UIController uiController;
    public class TranscriptionObjectData
    {
        public string id { get; set; }
    }

    public class TranscriptionResultData
    {
        public string status { get; set; }
        public string text { get; set; }
    }

    public class UrlData
    {
        public string upload_url { get; set; }
    }

    private void Start()
    {
        key = Resources.Load<TextAsset>("AssemblyKey").text;
        uiController = GetComponent<UIController>();
    }

    public IEnumerator StartTranscription(string url)
    {
        UnityWebRequest req = new UnityWebRequest("https://api.assemblyai.com/v2/transcript");
        req.SetRequestHeader("Content-Type", "application/json");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes("{\"audio_url\": \"" + url + "\" }"));
        req.SetRequestHeader("authorization", key);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.method = UnityWebRequest.kHttpVerbPOST;
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                TranscriptionObjectData jOb = JsonConvert.DeserializeObject<TranscriptionObjectData>(req.downloadHandler.text);
                uiController.ShowMessageAsDictation("Starting to think...");
                StartCoroutine(GetTranscriptionResult(jOb.id));
            }catch(Exception e)
            {
                uiController.ShowDictation("Error starting transcription request: " + e.Message);
            }
        }
        else
        {
            //Debug.Log("Failed: " + req.responseCode);
            uiController.ShowDictation("Error starting transcription request: " + req.downloadHandler.text);
        }
        req.Dispose();
    }

    public IEnumerator UploadFile()
    {
        UnityWebRequest req = new UnityWebRequest("https://api.assemblyai.com/v2/upload");
        req.uploadHandler = new UploadHandlerRaw(File.ReadAllBytes(Path.Combine(Application.persistentDataPath, "recording.wav")));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.method = UnityWebRequest.kHttpVerbPOST;
        req.SetRequestHeader("authorization", key);
        req.SetRequestHeader("Transfer-Encoding", "chunked");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                UrlData jOb = JsonConvert.DeserializeObject<UrlData>(req.downloadHandler.text);
                StartCoroutine(StartTranscription(jOb.upload_url));
            }catch(Exception e)
            {
                uiController.ShowDictation("Error uploading voice file: " + e.Message);
            }
        }
        else
        {
            uiController.ShowDictation("Error uploading voice file: " + req.responseCode);
        }
        GetComponent<VoiceCommandReceiver>().SetSafeToRecord();
        req.Dispose();
    }

    public IEnumerator GetTranscriptionResult(string id)
    {
        UnityWebRequest req = UnityWebRequest.Get("https://api.assemblyai.com/v2/transcript/" + id);
        req.SetRequestHeader("authorization", key);
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            TranscriptionResultData jOb;
            try
            {
                jOb = JsonConvert.DeserializeObject<TranscriptionResultData>(req.downloadHandler.text);
            }
            catch(Exception e)
            {
                uiController.ShowDictation("Error getting transcription result: "+e.Message);
                jOb = new TranscriptionResultData();
                jOb.status = "error";
            }
            uiController.ShowMessageAsDictation("Thinking...");
            if (jOb.status == "completed")
            {
                GetComponent<VoiceCommandReceiver>().ProcessDictation(jOb.text);
            }
            else if (jOb.status == "error")
            {
                uiController.ShowDictation("Transcription error");
            }
            else
            {
                yield return new WaitForSeconds(1);
                StartCoroutine(GetTranscriptionResult(id));
            }
        }
        else
        {
            Debug.Log("Failed: " + req.responseCode);
        }
        req.Dispose();
    }
}
