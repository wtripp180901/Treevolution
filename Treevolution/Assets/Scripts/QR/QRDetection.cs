using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.QR;
using TMPro;
using Microsoft.MixedReality.OpenXR;
using Unity.VisualScripting;

public class QRDetection : MonoBehaviour
{
    public TMP_Text debugText;
    public GameObject towerMarker;
    private QRCodeWatcher watcher;
    private SortedDictionary<System.Guid, (QRCode code, GameObject obj)> trackedCodes = new SortedDictionary<System.Guid, (QRCode Code, GameObject Obj)>();
    private System.Threading.Tasks.Task<QRCodeWatcherAccessStatus> accessRequester;
    private QRCode lastCode = null;
    private Pose lastPos = Pose.identity;
    private bool hasStarted = false;
    
    async void Start() {
        towerMarker.SetActive(false);
        Debug.Log("QR Script started");
        System.Threading.Tasks.Task<QRCodeWatcherAccessStatus> accessRequester = QRCodeWatcher.RequestAccessAsync();
        debugText.text = accessRequester.Status.ToString();
        await accessRequester;
    }


    // Update is called once per frame
    void Update()
    {
        debugText.text = accessRequester.Status.ToString();
        if (!accessRequester.IsCompleted)
        {
            debugText.text = "Please Accept Permissions [" + accessRequester.Result.ToString() + "]";
            return;
        }

        if (!hasStarted && accessRequester.Result == QRCodeWatcherAccessStatus.Allowed)
            InitialiseQR();
        
        if (hasStarted) {
            if (lastCode == null && watcher.GetList().Count == 0)
                debugText.text = "Scanning Started";
            else if (watcher.GetList().Count != trackedCodes.Count)
                syncWithWatcher();
            else
            {
                lock (trackedCodes)
                {
                    debugText.text = "Updating Markers\n" +
                        "Watchers: " + watcher.GetList().Count.ToString() + "\n" +
                        "Markers: " + trackedCodes.Count.ToString() + "\n" +
                        "Last: " + lastCode.Data + " @ " + lastPos.position.ToString();
                }
            }
        }
    }

    private IList<(QRCode, GameObject)> getCurrentList()
    {
        lock (trackedCodes)
        {
            return new List<(QRCode, GameObject)>(trackedCodes.Values);
        }
    }

    

    private void syncWithWatcher()
    {

        foreach (QRCode code in watcher.GetList())
        {
            lock (trackedCodes)
            {
                if (!trackedCodes.ContainsKey(code.Id))
                {
                    Pose position;
                    SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
                    float sideLength = code.PhysicalSideLength;
                    Vector3 markerSize = new Vector3(sideLength, sideLength, sideLength);
                    GameObject tempMarker = Instantiate(towerMarker);
                    tempMarker.transform.position = position.position;
                    tempMarker.transform.rotation = position.rotation;
                    tempMarker.transform.localScale = markerSize;
                    tempMarker.SetActive(true);

                    trackedCodes.Add(code.Id, (code, tempMarker));

                    this.lastCode = code;
                    this.lastPos = position;
                }
            }
        }        
    }

    private void codeUpdatedEventHandler(object sender, QRCodeUpdatedEventArgs args)
    {
        Pose position;
        SpatialGraphNode.FromStaticNodeId(args.Code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
        float sideLength = args.Code.PhysicalSideLength;
        Vector3 markerSize = new Vector3(sideLength, sideLength, sideLength);
        lock (trackedCodes)
        {
            trackedCodes[args.Code.Id].obj.transform.position = position.position;
            trackedCodes[args.Code.Id].obj.transform.rotation = position.rotation;
            trackedCodes[args.Code.Id].obj.transform.localScale = markerSize;
        }
        this.lastCode = args.Code;
        this.lastPos = position;
    }

    private void codeAddedEventHandler(object sender, QRCodeAddedEventArgs args)
    {
        Pose position;
        SpatialGraphNode.FromStaticNodeId(args.Code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
        float sideLength = args.Code.PhysicalSideLength;
        Vector3 markerSize = new Vector3(sideLength, sideLength, sideLength);
        GameObject tempMarker = Instantiate(towerMarker);
        tempMarker.SetActive(true);
        tempMarker.transform.position = position.position;
        tempMarker.transform.rotation = position.rotation;
        tempMarker.transform.localScale = markerSize;
        lock (trackedCodes)
        {
            trackedCodes.Add(args.Code.Id, (args.Code, tempMarker));
        }
        this.lastCode = args.Code;
        this.lastPos = position;
    }

    private void codeRemovedEventHandler(object sender, QRCodeRemovedEventArgs args)
    {
        debugText.text = "Removed";
        lock (trackedCodes)
        {
            if (trackedCodes.ContainsKey(args.Code.Id))
            {
                trackedCodes.Remove(args.Code.Id);
            }
        }
    }

    private void InitialiseQR()
    {
        if (QRCodeWatcher.IsSupported())
        {
            hasStarted = true;
            watcher = new QRCodeWatcher();
            watcher.Added += new System.EventHandler<QRCodeAddedEventArgs>(this.codeAddedEventHandler);
            watcher.Updated += new System.EventHandler<QRCodeUpdatedEventArgs>(this.codeUpdatedEventHandler);
            watcher.Removed += new System.EventHandler<QRCodeRemovedEventArgs>(this.codeRemovedEventHandler);
            watcher.Start();
            syncWithWatcher();
        }
        else
        {
            debugText.text = "QR Tracking Not Supported";
        }
    }
}
