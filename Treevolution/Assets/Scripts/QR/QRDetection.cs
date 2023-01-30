using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.QR;
using TMPro;
using Microsoft.MixedReality.OpenXR;

public class QRDetection : MonoBehaviour
{
    QRCodeWatcher watcher;
    TMP_Text debugText;
    GameObject debugMarker;
    
    async void Start()
    {
        debugMarker = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("QR Script started");
        debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<TMP_Text>();
        debugText.text = "tst";
        await QRCodeWatcher.RequestAccessAsync();
        Debug.Log(QRCodeWatcher.IsSupported());
        if (QRCodeWatcher.IsSupported())
        {
            watcher = new QRCodeWatcher();
            watcher.Added += codeAddedEventHandler;
            watcher.Updated += codeUpdatedEventHandler;
            watcher.Start();
            debugText.text = "started "+watcher.GetList().Count;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //debugText.text = "updt_code";
        if (QRCodeWatcher.IsSupported())
        {
            /*Pose position;
            SpatialGraphNode.FromDynamicNodeId(watcher.GetList()[0].SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
            debugMarker.transform.position = position.position;
            debugText.text = "S_" + watcher.GetList().Count.ToString() + "\n_" + watcher.GetList()[0].Data + "\n_"+position.position.ToString();*/
        }
        else
        {
            debugText.text = "U";
        }
    }

    void codeUpdatedEventHandler(object sender,QRCodeUpdatedEventArgs args)
    {
        /*Pose position;
        SpatialGraphNode.FromDynamicNodeId(args.Code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
        debugMarker.transform.position = position.position;*/
        //debugText.text = "S_\n_" + args.Code.Data + "\n_" + position.position.ToString();
        debugText.text = "updated";
    }

    void codeAddedEventHandler(object sender, QRCodeAddedEventArgs args)
    {
        /*Pose position;
        SpatialGraphNode.FromDynamicNodeId(args.Code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
        debugMarker.transform.position = position.position;*/
        //debugText.text = "S_\n_" + args.Code.Data + "\n_" + position.position.ToString();
        debugText.text = "added";
    }
}
