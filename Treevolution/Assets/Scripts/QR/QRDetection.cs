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
    
    async void Start()
    {
        debugMarker = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("QR Script started");
        debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<TMP_Text>();
        debugText.text = "tst";
        status = await QRCodeWatcher.RequestAccessAsync();
        Debug.Log(QRCodeWatcher.IsSupported());
        if (QRCodeWatcher.IsSupported() && status == QRCodeWatcherAccessStatus.Allowed)
        {
            watcher = new QRCodeWatcher();
            watcher.Added += codeAddedEventHandler;
            watcher.Updated += codeUpdatedEventHandler;
            watcher.Start();
            debugText.text = "- QR is Supported\n- Permissions Accepted\n- QRCodeWatcher Started";
            debugMarker.SetActive(false);
        }
        else if (status != QRCodeWatcherAccessStatus.Allowed){
            debugText.text = "<< Permissions Not Accepted >>";
        }
        else if (!QRCodeWatcher.IsSupported())
        {
            debugText.text = "<< QRCodeWatcher Not Supported >>";
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (QRCodeWatcher.IsSupported() && status == QRCodeWatcherAccessStatus.Allowed)
        {
            if (lastAdded == null) debugText.text = "Scanning";
            {
                Pose position;
                SpatialGraphNode.FromStaticNodeId(lastAdded.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
                debugMarker.transform.position = position.position;
                Vector3 markerPos = new Vector3(lastAdded.PhysicalSideLength, lastAdded.PhysicalSideLength, lastAdded.PhysicalSideLength);
                debugMarker.transform.SetPositionAndRotation(markerPos, position.rotation);
                debugMarker.SetActive(true);
                debugText.text = "Found: " + watcher.GetList().Count.ToString() + "\n" + lastAdded.Data + "\n" + position.position.ToString();
            }
        }
    }

    private void codeUpdatedEventHandler(object sender,QRCodeUpdatedEventArgs args)
    {
        this.lastAdded = args.Code;
    }

    private void codeAddedEventHandler(object sender, QRCodeAddedEventArgs args)
    {
        this.lastAdded = args.Code;
 
    }
}
