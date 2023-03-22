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
    public GameObject wallMarker;
    private QRCodeWatcher watcher;
    private SortedDictionary<System.Guid, (QRCode code, GameObject obj)> trackedCodes;
    private Queue<QRCode> updatedCodeQueue;
    private System.Threading.Tasks.Task<QRCodeWatcherAccessStatus> accessRequester;
    private (QRCode code, Pose pose) lastCode;
    private Pose oldPlanePose;
    private bool hasStarted;

    private bool planeCreated;
    private bool c1Set;
    private Vector3 cornerMarker1;


    private void initProperties()
    {
        planeMapper.ClearPlane();
        resetTrackedCodes();
        resetCodeQueue();
        hasStarted = false;
        lastCode = (null, Pose.identity);
        planeCreated = false;
        c1Set = false;
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
            {
                //debugText.text = "Scanning Started";
            }
            else
            {
                //debugText.text = trackedCodesCount + " / " + watcher.GetList().Count.ToString() + " : " + updatedCodeQueue.Count.ToString();
                lock (updatedCodeQueue)
                {
                    while (updatedCodeQueue.Count > 0)
                    {
                        QRCode code = updatedCodeQueue.Dequeue();
                        updateCodeHologram(code);
                        //debugText.text = "Code: " + lastCode.code.Data + " @ " + lastCode.pose.position;
                        drawPlane("C1", code);
                    }
                }
            }

        }
        else if (!QRCodeWatcher.IsSupported())
        {
            debugText.text = "QR Is Unsupported";
        }
    }



    private void drawPlane(string c1Data, QRCode code)
    {
        lock (trackedCodes)
        {
            Pose newPose;
            SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out newPose); // Get pose of QR Code
            if (code.Data.ToString() == c1Data)
            {
                newPose = tryGetNewCornerMarkerPose(newPose, c1Set, oldPlanePose);
                c1Set = true;
            }
            if (c1Set && !planeCreated)
            {
                planeMapper.CreateNewPlane(newPose);
                planeCreated = true;
                oldPlanePose = newPose;
            }
        }
    }

    public void spawnObjectOnQR(QRCode qr, GameObject obj)
    {
        Pose qrPose;
        SpatialGraphNode.FromStaticNodeId(qr.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out qrPose); // Get pose of QR Code
        float qrSideLength = qr.PhysicalSideLength;
        Vector3 markerSize = new Vector3(qrSideLength, qrSideLength, qrSideLength);
        Vector3 qrCentreWorld = qrPose.position + qrPose.right * qrSideLength / 2 + qrPose.up * qrSideLength / 2; // QR Centre in world coordinates
        obj.transform.right = qrPose.right;
        obj.transform.up = qrPose.forward;
        obj.transform.forward = -qrPose.up;
        obj.transform.position = qrCentreWorld;
        GameObject qrPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        qrPlane.transform.rotation = qrPose.rotation;
        qrPlane.transform.Rotate(qrPose.right, 90);
        qrPlane.transform.position = qrCentreWorld;
        qrPlane.transform.localScale = markerSize * 0.1f;
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
                bool scaleToMarker = false;
                Vector3 markerOffset = sideLength / 2 * (currentPose.right + currentPose.up); ;
                String[] data = updatedCode.Data.Split(' ');

                Quaternion rotation = Quaternion.LookRotation(-currentPose.up, currentPose.forward);
                switch (data[0])
                {
                    case "Tower":
                        if (tempMarker == null) markerType = towerMarker;
                        break;

                    case "Wall":
                        if (tempMarker == null)
                        {
                            markerType = wallMarker;
                            markerOffset += currentPose.forward * markerType.GetComponent<Collider>().transform.lossyScale.y / 2;
                        }
                        else
                        {
                            markerOffset += currentPose.forward * trackedCodes[updatedCode.Id].obj.GetComponent<Collider>().transform.lossyScale.y / 2;
                        }
                        rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
                        break;
                    default:
                        if (tempMarker == null) markerType = defaultMarker;
                        scaleToMarker = true;
                        break;
                }
                if (tempMarker == null)
                {
                    tempMarker = Instantiate(markerType, currentPose.position + markerOffset, rotation);
                    tempMarker.SetActive(true);
                }
                else
                {
                    tempMarker.transform.SetPositionAndRotation(currentPose.position + markerOffset, rotation);
                }

                if (scaleToMarker) tempMarker.transform.localScale = new Vector3(sideLength, sideLength * 0.2f, sideLength);

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

    Pose tryGetNewCornerMarkerPose(Pose newPose, bool cornerSet, Pose oldPose)
    {
        if (!cornerSet || Vector3.Distance(oldPose.position, newPose.position) > 0.01 || Math.Abs(Quaternion.Angle(oldPose.rotation, newPose.rotation)) > 5)
        {
            oldPose = newPose;
            planeCreated = false;
        }
        return oldPose;
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

    public void StopQR()
    {
        if (QRCodeWatcher.IsSupported()) watcher.Stop();
    }
}