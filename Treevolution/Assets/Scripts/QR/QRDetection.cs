using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.QR;
using TMPro;
using Microsoft.MixedReality.OpenXR;
using Unity.VisualScripting;

public class QRDetection : MonoBehaviour
{
    QRCodeWatcher watcher;
    TMP_Text debugText;
    GameObject debugMarker;
    QRCodeWatcherAccessStatus status;
    QRCode lastAdded = null;
    bool hasStarted = false;
    
    async void Start()
    {
        debugMarker = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("QR Script started");
        debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<TMP_Text>();
        debugText.text = "tst";
        status = await QRCodeWatcher.RequestAccessAsync();
        Debug.Log(QRCodeWatcher.IsSupported());
    }

    // Update is called once per frame
    void Update()
    {
        if (QRCodeWatcher.IsSupported() && status == QRCodeWatcherAccessStatus.Allowed && !hasStarted)
        {
            InitialiseQR();
        }
        else
        {
            debugText.text = "accept perms";
        }
        //debugText.text = "updt_code";
        if (QRCodeWatcher.IsSupported() && hasStarted)
        {
            if (lastAdded == null) debugText.text = "Scanning";
            else
            {
                Pose position;
                SpatialGraphNode.FromStaticNodeId(lastAdded.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
                debugMarker.transform.position = position.position;
                debugText.text = "added_" + watcher.GetList().Count.ToString() + "\n" + lastAdded.Data + "\n" + position.position.ToString();
            }
        }
        else if (!QRCodeWatcher.IsSupported())
        {
            debugText.text = "Unsupported or not started yet";
        }
    }

    private void codeUpdatedEventHandler(object sender,QRCodeUpdatedEventArgs args)
    {
        /*Pose position;
        SpatialGraphNode.FromDynamicNodeId(args.Code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
        debugMarker.transform.position = position.position;*/
        //debugText.text = "S_\n_" + args.Code.Data + "\n_" + position.position.ToString();
        debugText.text = "updated";
    }

    private void codeAddedEventHandler(object sender, QRCodeAddedEventArgs args)
    {
        this.lastAdded = args.Code; // Doesn't seem to work?
    }

    private void InitialiseQR()
    {
        hasStarted = true;
        watcher = new QRCodeWatcher();
        watcher.Added += codeAddedEventHandler;
        watcher.Updated += codeUpdatedEventHandler;
        watcher.Start();
        debugText.text = "started " + watcher.GetList().Count;
    }
}
