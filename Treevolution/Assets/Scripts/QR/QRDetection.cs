using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Runs QR Detection via MRTK's QRCodeWatcher.
/// </summary>
public class QRDetection : MonoBehaviour
{
    /// <summary>
    /// TextMeshPro Game Object to display debug information on.
    /// </summary>
    public TMP_Text debugText;
    /// <summary>
    /// GameObject which is placed on the QR Code marking the corner of the game board.
    /// </summary>
    public GameObject planeMarker;
    /// <summary>
    /// GameObject which is placed on a tower marker.
    /// </summary>
    public GameObject towerMarker;
    /// <summary>
    /// GameObject which is placed on a wall marker.
    /// </summary>
    public GameObject wallMarker;
    /// <summary>
    /// Indicates whether the plane should lock and stop searching for plane marker updates.
    /// </summary>
    [HideInInspector]
    public bool lockPlane;

    /// <summary>
    /// Running PlaneMapper instance.
    /// </summary>
    private PlaneMapper _planeMapper;
    /// <summary>
    /// An instance of QRCodeWatcher.
    /// </summary>
    private QRCodeWatcher _qRCodeWatcher;
    /// <summary>
    /// All currently tracked QR codes, and the GameObjects placed on them (indexed by their QRCode.Id).
    /// </summary>
    private SortedDictionary<System.Guid, (QRCode code, GameObject obj)> _trackedCodes;
    /// <summary>
    /// Queue for QR code updates.
    /// </summary>
    private Queue<QRCode> _updatedCodeQueue;
    /// <summary>
    /// Used to request relevant QRCodeWatcher permissions.
    /// </summary>
    private System.Threading.Tasks.Task<QRCodeWatcherAccessStatus> _accessRequester;
    /// <summary>
    /// The previous plane pose for checking if a new plane pose is "different enough" for it to update.
    /// </summary>
    private Pose _oldPlanePose;
    /// <summary>
    /// Indicates whether the QR Detector is running.
    /// </summary>
    private bool _running;
    /// <summary>
    /// Indicates if the plane has been created.
    /// </summary>
    private bool _planeCreated;
    /// <summary>
    /// Indicates if the plane marker has been set.
    /// </summary>
    private bool _planeMarkerSet;
    /// <summary>
    /// Indicates if the Update code is running for the first time or not.
    /// </summary>
    private bool _initialRun = true;


    private void initProperties()
    {
        _planeMapper.ClearPlane();
        resetTrackedCodes();
        resetCodeQueue();
        _running = false;
        _planeCreated = false;
        _planeMarkerSet = false;
    }

    async void Start()
    {
        _accessRequester = QRCodeWatcher.RequestAccessAsync();
        if (_accessRequester.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
        {
            await _accessRequester;
        }
        initProperties();
    }

    // Update is called once per frame
    void Update()
    {
        // If it is running for the first time, and the permissions have all been accepted ...
        if (_initialRun && !_running && (_accessRequester.IsCompletedSuccessfully && _accessRequester.Result == QRCodeWatcherAccessStatus.Allowed))
            InitialiseQR(); 
        else if (_running) // Otherwise, if it has started running already ...
        {
            int trackedCodesCount = 0;
            lock (_trackedCodes) { trackedCodesCount = _trackedCodes.Count; } // Find number of currently tracked codes
            lock (_updatedCodeQueue)
            {
                while (_updatedCodeQueue.Count > 0) // Whilst there are code updates to be processed
                {
                    QRCode code = _updatedCodeQueue.Dequeue();
                    if (lockPlane && code.Data == "C1") // If plane marker and plane is locked, ignore it
                        continue;
                    else if (!lockPlane && code.Data == "C1")
                        drawPlane("C1", code); // Draw the plane
                    
                    updateCodeHologram(code); // Update the hologram on the code
                }
            }

        }
    }


    /// <summary>
    /// Checks and draws the game board plane.
    /// </summary>
    /// <param name="c1Data">Data that should be displayed on the QRCode of the plane marker.</param>
    /// <param name="code">Object representing a potential plane marker.</param>
    private void drawPlane(string c1Data, QRCode code)
    {
        lock (_trackedCodes)
        {
            Pose newPose;
            SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out newPose); // Get pose of QR Code
            if (code.Data.ToString() == c1Data)
            {
                newPose = tryGetNewCornerMarkerPose(newPose, _planeMarkerSet, _oldPlanePose);
                _planeMarkerSet = true;
            }
            if (_planeMarkerSet && !_planeCreated)
            {
                _planeMapper.CreateNewPlane(newPose);
                _planeCreated = true;
                _oldPlanePose = newPose;
            }
        }
    }

    /// <summary>
    /// Spawns a GameObject on a QRCode, and covers the QR with a plane.
    /// </summary>
    /// <param name="qr">QrCode to spawn the object on.</param>
    /// <param name="obj">GameObject to spawn.</param>
    public GameObject spawnObjectOnQR(QRCode qr, GameObject obj)
    {
        Pose qrPose;
        SpatialGraphNode.FromStaticNodeId(qr.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out qrPose); // Get pose of QR Code
        float qrSideLength = qr.PhysicalSideLength;
        Vector3 markerSize = new Vector3(qrSideLength, qrSideLength, qrSideLength);
        Vector3 qrCentreWorld = qrPose.position + qrSideLength / 2 * (qrPose.right + qrPose.up);// QR Centre in World Coordinates
        obj.transform.rotation = Quaternion.LookRotation(-qrPose.up, qrPose.forward);
        obj.transform.position = qrCentreWorld;
        GameObject qrPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        qrPlane.transform.rotation = Quaternion.LookRotation(-qrPose.up, qrPose.forward); ;
        qrPlane.transform.position = qrCentreWorld;
        qrPlane.transform.localScale = markerSize * 0.1f;
        qrPlane.tag = "Floor";
        qrPlane.transform.SetParent(obj.transform);
        return obj;
    }

    private void updateCodeHologram(QRCode qrCode)
    {
        lock (_trackedCodes)
        {
            if (_trackedCodes.ContainsKey(qrCode.Id))
            {
                Pose currentPose;
                SpatialGraphNode.FromStaticNodeId(qrCode.SpatialGraphNodeId).TryLocate(FrameTime.OnUpdate, out currentPose); // Get pose of QR Code
                float sideLength = qrCode.PhysicalSideLength;
                Vector3 markerSize = new Vector3(sideLength, sideLength, sideLength);
                GameObject tempMarker = _trackedCodes[qrCode.Id].obj;
                GameObject markerType = null;
                bool scaleToMarker = false;
                Vector3 markerOffset = sideLength / 2 * (currentPose.right + currentPose.up); ;
                String[] data = qrCode.Data.Split(' ');

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
                            markerOffset += currentPose.forward * _trackedCodes[qrCode.Id].obj.GetComponent<Collider>().transform.lossyScale.y / 2;
                        }
                        rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
                        break;
                    default:
                        if (tempMarker == null) markerType = planeMarker;
                        scaleToMarker = true;
                        break;
                }
                /*AdDED
                if(markerType == towerMarker)
                {
                    if (tempMarker == null)
                    {
                        tempMarker = spawnObjectOnQR(qrCode, markerType);
                        Instantiate(tempMarker);
                    }
                    else
                    {
                        tempMarker.transform.SetPositionAndRotation(currentPose.position + markerOffset, rotation);
                    }
                }*/
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

                _trackedCodes[qrCode.Id] = (qrCode, tempMarker);
            }
        }
    }

    private Pose tryGetNewCornerMarkerPose(Pose newPose, bool cornerSet, Pose oldPose)
    {
        if (!cornerSet || Vector3.Distance(oldPose.position, newPose.position) > 0.03 || Math.Abs(Quaternion.Angle(oldPose.rotation, newPose.rotation)) > 5)
        {
            oldPose = newPose;
            _planeCreated = false;
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
        lock (_updatedCodeQueue)
        {
            _updatedCodeQueue.Enqueue(args.Code);
        }
        lock (_trackedCodes)
        {
            if (!_trackedCodes.ContainsKey(args.Code.Id))
                _trackedCodes[args.Code.Id] = (args.Code, null);
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
        lock (_updatedCodeQueue)
        {
            _updatedCodeQueue.Enqueue(args.Code);
        }

        lock (_trackedCodes)
        {
            if (!_trackedCodes.ContainsKey(args.Code.Id))
                _trackedCodes[args.Code.Id] = (args.Code, null);
        }
    }

    private void removedCodeEvent(object sender, QRCodeRemovedEventArgs args)
    {
        lock (_trackedCodes)
        {
            Destroy(_trackedCodes[args.Code.Id].obj);
            _trackedCodes.Remove(args.Code.Id);
        }
    }

    private void InitialiseQR()
    {
        if (QRCodeWatcher.IsSupported())
        {
            _running = true;
            _qRCodeWatcher = new QRCodeWatcher();
            _qRCodeWatcher.Added += new System.EventHandler<QRCodeAddedEventArgs>(this.addedCodeEvent);
            _qRCodeWatcher.Updated += new System.EventHandler<QRCodeUpdatedEventArgs>(this.updatedCodeEvent);
            _qRCodeWatcher.Removed += new System.EventHandler<QRCodeRemovedEventArgs>(this.removedCodeEvent);
            foreach (Guid id in _trackedCodes.Keys)
            {
                Destroy(_trackedCodes[id].obj);
                _trackedCodes.Remove(id);
            }
            _qRCodeWatcher.Start();
        }
        else
        {
            debugText.text = "QR Tracking Not Supported";
        }
        _initialRun = false;
    }

    private void resetTrackedCodes()
    {
        if (_trackedCodes.IsNotNull())
        {
            lock (_trackedCodes)
            {
                foreach ((QRCode code, GameObject obj) item in _trackedCodes.Values)
                {
                    Destroy(item.obj);
                }
                _trackedCodes.Clear();
            }
        }
        else
        {
            _trackedCodes = new SortedDictionary<Guid, (QRCode code, GameObject obj)>();
        }
    }


    private void resetCodeQueue()
    {
        if (_updatedCodeQueue.IsNotNull())
        {
            lock (_updatedCodeQueue) { _updatedCodeQueue.Clear(); }
        }
        else
        {
            _updatedCodeQueue = new Queue<QRCode>();
        }
    }

    /// <summary>
    /// Halts QR Detection.
    /// </summary>
    public void StopQR()
    {
        //if (QRCodeWatcher.IsSupported()) watcher.Stop();
        _running = false;
    }
    /// <summary>
    /// Starts QR Detection.
    /// </summary>
    public void StartQR()
    {
        resetCodeQueue();
        //if (QRCodeWatcher.IsSupported()) watcher.Start();
        _running = true;
    }
}