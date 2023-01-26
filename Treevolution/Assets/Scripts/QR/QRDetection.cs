using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.QR;
using TMPro;

public class QRDetection : MonoBehaviour
{
    //QRCodeWatcher watcher;
    TMP_Text debugText;
    
    async void Start()
    {
        debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<TMP_Text>();
        debugText.text = "tst";
        await QRCodeWatcher.RequestAccessAsync();
        //Debug.Log(QRCodeWatcher.IsSupported());
        //watcher = new QRCodeWatcher();
        //watcher.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (QRCodeWatcher.IsSupported())
        {
            //debugText.text = watcher.GetList().Count.ToString();
            debugText.text = "QR supported";
        }
        else
        {
            debugText.text = "QR not supported";
        }
    }
}
