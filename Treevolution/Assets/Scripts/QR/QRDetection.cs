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
<<<<<<< HEAD
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

=======
>>>>>>> 8966f4127c5afe111b9391b8923e1faa604550ac
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        if (QRCodeWatcher.IsSupported() && status == QRCodeWatcherAccessStatus.Allowed)
=======
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
>>>>>>> 8966f4127c5afe111b9391b8923e1faa604550ac
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
<<<<<<< HEAD
=======
        else if (!QRCodeWatcher.IsSupported())
        {
            debugText.text = "Unsupported or not started yet";
        }
>>>>>>> 8966f4127c5afe111b9391b8923e1faa604550ac
    }

    private void codeUpdatedEventHandler(object sender,QRCodeUpdatedEventArgs args)
    {
        this.lastAdded = args.Code;
    }

    private void codeAddedEventHandler(object sender, QRCodeAddedEventArgs args)
    {
        this.lastAdded = args.Code;
 
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
