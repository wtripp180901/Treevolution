using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.QR;
using TMPro;
using Microsoft.MixedReality.OpenXR;

using System;

using Microsoft.MixedReality.Toolkit;
//using Microsoft.MixedReality.Toolkit.Utilities;
//HandPoseUtils.CalculateIndexPinch();



public class QRDetection : MonoBehaviour 
{
    public TMP_Text debugText;
    public PlaneMapper planeMapper;
    public GameObject defaultMarker;
    public GameObject towerMarker;
    private QRCodeWatcher watcher;
    private SortedDictionary<System.Guid, (QRCode code, GameObject obj)> trackedCodes;
    private Queue<QRCode> updatedCodeQueue;
    private System.Threading.Tasks.Task<QRCodeWatcherAccessStatus> accessRequester;
    private (QRCode code, Pose pose) lastCode;
    private bool hasStarted;

    private bool planeCreated;
    private bool c1Set;
    private bool c2Set;
    private Vector3 cornerMarker1;
    private Vector3 cornerMarker2;


    private void initProperties()
    {
        planeMapper.ClearPlane();
        resetTrackedCodes();
        resetCodeQueue();
        hasStarted = false;
        lastCode = (null, Pose.identity);
        planeCreated = false;
        c1Set = false;
        c2Set = false;
    }

    async void Start()
    {
        accessRequester = QRCodeWatcher.RequestAccessAsync();
        if (accessRequester.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
        {
            await accessRequester;
        }
        initProperties();
    }

    // Update is called once per frame
    void Update()
    {
        //debugText.text = "Update";
        if (!hasStarted && (accessRequester.IsCompletedSuccessfully && accessRequester.Result == QRCodeWatcherAccessStatus.Allowed))
            InitialiseQR();

        else if (hasStarted)
        {
            int trackedCodesCount = 0;
            lock (trackedCodes) { trackedCodesCount = trackedCodes.Count; }

            if (lastCode.code == null && trackedCodesCount == 0)
                debugText.text = "Scanning Started";

            else
            {
                //debugText.text = trackedCodesCount + " / " + watcher.GetList().Count.ToString() + " : " + updatedCodeQueue.Count.ToString();
                lock (updatedCodeQueue)
                {
                    while (updatedCodeQueue.Count > 0)
                    {
                        QRCode code = updatedCodeQueue.Dequeue();
                        updateCodeHologram(code);
                        debugText.text = "Code: " + lastCode.code.Data + " @ " + lastCode.pose.position;
                        drawPlane("C1", "C2", code);
                    }
                }
            }

        }
        else if (!QRCodeWatcher.IsSupported())
        {
            debugText.text = "QR Is Unsupported";
        }
    }



    private void drawPlane(string c1Data, string c2Data, QRCode code)
    {
        lock (trackedCodes)
        {
            if (code.Data.ToString() == c1Data)
            {
                cornerMarker1 = tryGetNewCornerMarkerPosition(trackedCodes[code.Id].obj.transform.position, c1Set, cornerMarker1);
                c1Set = true;
            }
            else if (code.Data.ToString() == c2Data)
            {
                cornerMarker2 = tryGetNewCornerMarkerPosition(trackedCodes[code.Id].obj.transform.position, c2Set, cornerMarker2);
                c2Set = true;
            }
            if (c1Set && c2Set && !planeCreated)
            {
                planeMapper.CreateNewPlane(cornerMarker1, cornerMarker2);
                planeCreated = true;
                debugText.text = "planeCreated: " + planeCreated + "\nc1: " + c1Set + " " + cornerMarker1 + "\nc2: " + c2Set + " " + cornerMarker2;

            }
        }
    }

    private void updateCodeHologram(QRCode updatedCode)
    {
        lock (trackedCodes)
        {
            if (trackedCodes.ContainsKey(updatedCode.Id))
            {
                Pose currentPose;
                SpatialGraphNode.FromStaticNodeId(updatedCode.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out currentPose); // Get pose of QR Code
                float sideLength = updatedCode.PhysicalSideLength;
                Vector3 markerSize = new Vector3(sideLength, sideLength, sideLength);
                GameObject tempMarker = trackedCodes[updatedCode.Id].obj;
                GameObject markerType = null;
                if (tempMarker == null)
                {
                    switch (updatedCode.Data)
                    {
                        case "Tower":
                            markerType = towerMarker;
                            break;
                        default:
                            markerType = defaultMarker;
                            break;
                    }
                    tempMarker = Instantiate(markerType);
                    tempMarker.SetActive(true);
                }
                Vector3 markerOffset = Vector3.zero;// sideLength / 2 * Vector3.up;
                tempMarker.transform.SetPositionAndRotation(currentPose.position + markerOffset, currentPose.rotation);
                tempMarker.transform.localScale = markerSize;

                trackedCodes[updatedCode.Id] = (updatedCode, tempMarker);
                this.lastCode = (updatedCode, currentPose);
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

    Vector3 tryGetNewCornerMarkerPosition(Vector3 newPos, bool cornerSet, Vector3 oldMarkerPos)
    {
        if (!cornerSet || (oldMarkerPos - newPos).magnitude > 0.01)
        {
            oldMarkerPos = newPos;
            planeCreated = false;
        }
        return oldMarkerPos;
    }

    private void updatedCodeEvent(object sender, QRCodeUpdatedEventArgs args)
    {
        Pose currentPose;
        SpatialGraphNode.FromStaticNodeId(args.Code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out currentPose);
        if (currentPose == Pose.identity)
        {
            return; // Disregards
        }
        lock (updatedCodeQueue)
        {
            updatedCodeQueue.Enqueue(args.Code);
        }
        lock (trackedCodes)
        {
            if (!trackedCodes.ContainsKey(args.Code.Id))
            {
                trackedCodes[args.Code.Id] = (args.Code, null);
            }
        }
    }

    private void addedCodeEvent(object sender, QRCodeAddedEventArgs args)
    {
        Pose currentPose;
        SpatialGraphNode.FromStaticNodeId(args.Code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out currentPose);
        if(currentPose == Pose.identity)
        {
            return; // Disregards
        }

        lock (updatedCodeQueue) {
            updatedCodeQueue.Enqueue(args.Code);
        }

        lock (trackedCodes)
        {
            trackedCodes[args.Code.Id] = (args.Code, null);
        }
    }

    private void removedCodeEvent(object sender, QRCodeRemovedEventArgs args)
    {
        lock (trackedCodes)
        {
            Destroy(trackedCodes[args.Code.Id].obj);
            trackedCodes.Remove(args.Code.Id);
        }
    }

    private void InitialiseQR()
    {
        if (QRCodeWatcher.IsSupported())
        {
            hasStarted = true;
            watcher = new QRCodeWatcher();
            watcher.Added += new System.EventHandler<QRCodeAddedEventArgs>(this.addedCodeEvent);
            watcher.Updated += new System.EventHandler<QRCodeUpdatedEventArgs>(this.updatedCodeEvent);
            watcher.Removed += new System.EventHandler<QRCodeRemovedEventArgs>(this.removedCodeEvent);
            foreach (Guid id in trackedCodes.Keys)
            {
                Destroy(trackedCodes[id].obj);
                trackedCodes.Remove(id);
            }
            watcher.Start();
        }
        else
        {
            debugText.text = "QR Tracking Not Supported";
        }
    }

    private void resetTrackedCodes()
    {
        if (trackedCodes.IsNotNull())
        {
            lock (trackedCodes)
            {
                foreach ((QRCode code, GameObject obj) item in trackedCodes.Values)
                {
                    Destroy(item.obj);
                }
                trackedCodes.Clear();
            }
        }
        else
        {
            trackedCodes = new SortedDictionary<Guid, (QRCode code, GameObject obj)>();
        }
    }


    private void resetCodeQueue()
    {
        if (updatedCodeQueue.IsNotNull())
        {
            lock (updatedCodeQueue) { updatedCodeQueue.Clear(); }
        }
        else
        {
            updatedCodeQueue = new Queue<QRCode>();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasStarted && hasFocus)
        {
            initProperties();
            hasStarted = true;
        }
    }
}