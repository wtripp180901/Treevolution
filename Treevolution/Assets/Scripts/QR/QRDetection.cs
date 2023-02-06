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
    public PlaneMapper planeMapper;
    public GameObject towerMarker;
    private QRCodeWatcher watcher;
    //private List<(QRCode, GameObject)> markers;
    private SortedDictionary<System.Guid, (QRCode, GameObject)> trackedCodes = new SortedDictionary<System.Guid, (QRCode, GameObject)>();
    private QRCodeWatcherAccessStatus status = QRCodeWatcherAccessStatus.NotDeclaredByApp;
    private QRCode lastAdded = null;
    private bool hasStarted = false;
    
    async void Start() {
        towerMarker.SetActive(false);
        Debug.Log("QR Script started");
        status = await QRCodeWatcher.RequestAccessAsync();
        Debug.Log(QRCodeWatcher.IsSupported());
        debugText.text = "Start() Ran";
    }

    bool planeCreated = false;
    bool c1Set = false;
    bool c2Set = false;
    Vector3 cornerMarker1;
    Vector3 cornerMarker2;

    // Update is called once per frame
    void Update()
    {
        //debugText.text = "planeCreated: "+planeCreated+"\nc1: "+c1Set+" "+cornerMarker1+"\nc2: "+c2Set+" "+cornerMarker2;
        if (QRCodeWatcher.IsSupported() && !hasStarted && status == QRCodeWatcherAccessStatus.Allowed)
            InitialiseQR();
        else if (hasStarted) {
            if (watcher.GetList().Count != trackedCodes.Count){
                trackWatcherCodes();
            }
            if (lastAdded == null && watcher.GetList().Count == 0)
                debugText.text = "Scanning Started"; 
            else if (lastAdded != null) {
                debugText.text = "Updating Markers\nWatchers: " + watcher.GetList().Count.ToString() + "\nMarkers: " + trackedCodes.Count.ToString() + "\nLast: " + lastAdded.Data;
                lock (trackedCodes)
                {
                    foreach (System.Guid id in trackedCodes.Keys) // Maybe send as async routines?
                    {
                        Pose position;
                        SpatialGraphNode.FromStaticNodeId(trackedCodes[id].Item1.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
                        if (trackedCodes[id].Item1.Data != "C1" && trackedCodes[id].Item1.Data != "C2")
                        {
                            trackedCodes[id].Item2.transform.position = position.position;
                            float sideLength = trackedCodes[id].Item1.PhysicalSideLength;
                            Vector3 markerSize = new Vector3(sideLength, sideLength, sideLength);
                            trackedCodes[id].Item2.transform.rotation = position.rotation;
                            trackedCodes[id].Item2.transform.localScale = markerSize;
                        }else if(trackedCodes[id].Item1.Data == "C1")
                        {
                            cornerMarker1 = tryGetNewCornerMarkerPosition(position.position, c1Set, cornerMarker1);
                            c1Set = true;
                        }
                        else if (trackedCodes[id].Item1.Data == "C2")
                        {
                            cornerMarker2 = tryGetNewCornerMarkerPosition(position.position, c2Set, cornerMarker2);
                            c2Set = true;
                        }
                        if (c1Set && c2Set && !planeCreated)
                        {
                            planeMapper.CreateNewPlane(cornerMarker1, cornerMarker2);
                            planeCreated = true;
                        }
                    }
                }
            }
        }
        else if (!QRCodeWatcher.IsSupported())
        {
            debugText.text = "Unsupported or not started yet";
        }
    }

    private IList<(QRCode, GameObject)> getCurrentList()
    {
        lock (trackedCodes)
        {
            return new List<(QRCode, GameObject)>(trackedCodes.Values);
        }
    }

    Vector3 tryGetNewCornerMarkerPosition(Vector3 newPos,bool cornerSet,Vector3 oldMarkerPos)
    {
        if (!cornerSet || (oldMarkerPos - newPos).magnitude > 0.01)
        {
            oldMarkerPos = newPos;
            planeCreated = false;
        }
        return oldMarkerPos;
    }

    private void trackWatcherCodes()
    {
        lock (trackedCodes)
        {
            foreach (QRCode code in watcher.GetList())
            {
                GameObject tempMarker = Instantiate(towerMarker);
                tempMarker.SetActive(true);
                trackedCodes[code.Id] = (code, tempMarker);
            }
            this.lastAdded = watcher.GetList()[watcher.GetList().Count - 1];
        }
        
    }

    private void codeUpdatedEventHandler(object sender, QRCodeUpdatedEventArgs args)
    {
        debugText.text = "Updated";
        lock (trackedCodes)
        {
            trackedCodes[args.Code.Id] = (args.Code, trackedCodes[args.Code.Id].Item2);
        }
        this.lastAdded = args.Code;
    }

    private void codeAddedEventHandler(object sender, QRCodeAddedEventArgs args)
    {
        debugText.text = "Added";
        lock (trackedCodes)
        {
            GameObject tempMarker = Instantiate<GameObject>(towerMarker);
            tempMarker.SetActive(true);
            trackedCodes[args.Code.Id] = (args.Code, tempMarker);
        }
        this.lastAdded = args.Code;
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
        hasStarted = true;
        watcher = new QRCodeWatcher();
        watcher.Added += new System.EventHandler<Microsoft.MixedReality.QR.QRCodeAddedEventArgs>(this.codeAddedEventHandler);
        watcher.Updated += new System.EventHandler<Microsoft.MixedReality.QR.QRCodeUpdatedEventArgs>(this.codeUpdatedEventHandler);
        watcher.Removed += codeRemovedEventHandler;
        watcher.Start();
        debugText.text = "started " + watcher.GetList().Count;
    }
}
