using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.QR;
using TMPro;

public class QRDetection : MonoBehaviour
{
    QRCodeWatcher watcher;
    TMP_Text debugText;
    
    async void Start()
    {
        Debug.Log("QR Script started");
        debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<TMP_Text>();
        debugText.text = "tst";
        await QRCodeWatcher.RequestAccessAsync();
        Debug.Log(QRCodeWatcher.IsSupported());
        watcher = new QRCodeWatcher();
        watcher.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = "updt_code";
        if (QRCodeWatcher.IsSupported())
        {
            debugText.text = "S_" + watcher.GetList().Count.ToString() + "\n_" + watcher.GetList()[0].Data;
        }
        else
        {
            debugText.text = "U";
        }
    }
}
