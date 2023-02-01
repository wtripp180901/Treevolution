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
    public GameObject towerMarker;
    List<(QRCode, GameObject)> markers;
    QRCodeWatcherAccessStatus status;
    QRCode lastAdded = null;
    bool hasStarted = false;
    
    async void Start() {
        towerMarker.SetActive(false);
        Debug.Log("QR Script started");
        debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<TMP_Text>();
        debugText.text = "Start() Test Code";
        status = await QRCodeWatcher.RequestAccessAsync();
        Debug.Log(QRCodeWatcher.IsSupported());
    }

    // Update is called once per frame
    void Update()
    {
        if (QRCodeWatcher.IsSupported() && status == QRCodeWatcherAccessStatus.Allowed && !hasStarted)
            InitialiseQR();
        else
            debugText.text = "Please Accept Permissions\nor Terminate and Restart the App";

        if (hasStarted) {
            if (lastAdded == null) 
                debugText.text = "Scanning";
            else {
                foreach ((QRCode, GameObject) marker in markers) // Maybe send as async routines?
                {
                    Pose position;
                    SpatialGraphNode.FromStaticNodeId(marker.Item1.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
                    marker.Item2.transform.position = position.position;
                    float length = marker.Item1.PhysicalSideLength;
                    Vector3 markerPos = new Vector3(length, length, length);
                    marker.Item2.transform.SetPositionAndRotation(markerPos, position.rotation);
                }
                debugText.text = "Found: " + watcher.GetList().Count.ToString() + "\nLast Added: " + lastAdded.Data;

            }
        }
        else if (!QRCodeWatcher.IsSupported())
        {
            debugText.text = "Unsupported or not started yet";
        }
    }

    private void codeUpdatedEventHandler(object sender, QRCodeUpdatedEventArgs args)
    {
        this.lastAdded = args.Code;
        for (int i = 0; i < markers.Count; i++) {
            if (markers[i].Item1.Data == args.Code.Data)
            {
                markers[i] = (args.Code, markers[i].Item2);
                break;
            }
        }
    }

    private void codeAddedEventHandler(object sender, QRCodeAddedEventArgs args)
    {
        this.lastAdded = args.Code;
        GameObject tempMarker = Instantiate<GameObject>(towerMarker);
        tempMarker.SetActive(true);
        markers.Add((args.Code, tempMarker));
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
