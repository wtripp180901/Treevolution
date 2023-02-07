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
    private SortedDictionary<System.Guid, (QRCode code, GameObject obj)> trackedCodes = new SortedDictionary<System.Guid, (QRCode Code, GameObject Obj)>();
    private System.Threading.Tasks.Task<QRCodeWatcherAccessStatus> accessRequester;
    private QRCode lastCode = null;
    private Pose lastPos = Pose.identity;
    private bool hasStarted = false;
    
    async void Start() {
        towerMarker.SetActive(false);
        Debug.Log("QR Script started");
        accessRequester = QRCodeWatcher.RequestAccessAsync();
        debugText.text = accessRequester.Status.ToString();
        await accessRequester;
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
        if (accessRequester.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
        {
            debugText.text = debugText.text = accessRequester.Status.ToString();
        }
        else if (!hasStarted && (accessRequester.IsCompletedSuccessfully && accessRequester.Result == QRCodeWatcherAccessStatus.Allowed))
            InitialiseQR();
        
        else if (hasStarted) {
            if (lastCode == null && watcher.GetList().Count == 0)
                debugText.text = "Scanning Started";
            else if (watcher.GetList().Count != trackedCodes.Count)
                syncWithWatcher();
            else
            {
                lock (trackedCodes)
                {
                    if (trackedCodes[id].Item1.Data == "C1")
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
                    foreach (System.Guid id in trackedCodes.Keys) // Maybe send as async routines?
                    {
                        Pose position;
                        SpatialGraphNode.FromStaticNodeId(trackedCodes[id].Item1.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out position);
                        trackedCodes[id].Item2.transform.position = position.position;
                        float sideLength = trackedCodes[id].Item1.PhysicalSideLength;
                        Vector3 markerSize = new Vector3(sideLength, sideLength, sideLength);
                        trackedCodes[id].Item2.transform.rotation = position.rotation;
                        trackedCodes[id].Item2.transform.localScale = markerSize;
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
