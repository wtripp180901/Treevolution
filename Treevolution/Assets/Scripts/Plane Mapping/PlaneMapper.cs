using UnityEngine;

/// <summary>
/// Instantiates the playspace based on real world data
/// </summary>
public class PlaneMapper : MonoBehaviour
{
    /// <summary>
    /// The prefab which indicates the edges of the plane
    /// </summary>
    public GameObject planeMarker;
    /// <summary>
    /// The floor of the playspace
    /// </summary>
    public GameObject floor;
    /// <summary>
    /// The number of planeMarkers to be spawned on each side of the playspace's perimeter
    /// </summary>
    public int markerCount = 2;
    /// <summary>
    /// The central tree GameObject
    /// </summary>
    public GameObject treeModel;
    /// <summary>
    /// The width of the playspace
    /// </summary>
    public float tableWidth = 344;
    /// <summary>
    /// The depth of the playspace
    /// </summary>
    public float tableDepth = 163;

    /// <summary>
    /// Currently active GameStateManager instance.
    /// </summary>
    private GameStateManager _gameStateManager;
    /// <summary>
    /// Currently active QRDetection Instance.
    /// </summary>
    private QRDetection _qrDetection;

    private bool planeIsMapped = false;
    private Vector3 tl;
    public Vector3 topLeft { get { return tl; } }
    private Vector3 tr;
    public Vector3 topRight { get { return tr; } }
    private Vector3 bl;
    public Vector3 bottomLeft { get { return bl; } }
    private Vector3 br;
    public Vector3 bottomRight { get { return br; } }
    private float _minY;
    public float floorHeight { get { return _minY; } }
    private Pose _pose;
    public Pose pose { get { return _pose; } }

    public void SetupForTest(bool planeMapped)
    {
        gameObject.AddComponent<QRDetection>();
        gameObject.AddComponent<RealWorldPropertyMapper>();
        GetComponent<RealWorldPropertyMapper>().SetupForTest(this);
        _qrDetection = GetComponent<QRDetection>();
        planeIsMapped = planeMapped;
    }

    private void Start()
    {
        _gameStateManager = GetComponent<GameStateManager>();
        _qrDetection = GetComponent<QRDetection>();
    }

    /// <summary>
    /// Instantiates objects marking out the playspace based on a QR marker and maps the properties of the playspace in GameProperties
    /// </summary>
    /// <param name="marker">Pose of the the QR calibration marker from QRDetection</param>
    public void CreateNewPlane(Pose marker)
    {
        if (_qrDetection != null && _qrDetection.lockPlane)
        {
            return;
        }

        marker.rotation = Quaternion.LookRotation(marker.up, marker.forward);
        marker.rotation = Quaternion.Euler(0, marker.rotation.eulerAngles.y, 0);
        _pose = marker;

        _minY = marker.position.y;
        ClearPlane();

        bl = marker.position;
        br = marker.position + _pose.forward * tableWidth * 0.01f;
        tl = marker.position - _pose.right * tableDepth * 0.01f;
        tr = marker.position + _pose.forward * tableWidth * 0.01f - _pose.right * tableDepth * 0.01f;

        tl.y = bl.y;
        br.y = bl.y;
        tr.y = bl.y;



        Debug.DrawLine(bl, bl + Vector3.up, Color.green, 1000);
        Instantiate(planeMarker, tl, Quaternion.identity);
        Instantiate(planeMarker, tr, Quaternion.identity);
        Instantiate(planeMarker, bl, Quaternion.identity);
        Instantiate(planeMarker, br, Quaternion.identity);

        if (markerCount > 0)
        {
            Vector3 depthStep = (tl - bl) / (markerCount + 1);
            Vector3 widthStep = (br - bl) / (markerCount + 1);
            for (int i = 0; i < markerCount; i++)
            {
                Instantiate(planeMarker, bl + ((i + 1) * depthStep), Quaternion.identity);
                Instantiate(planeMarker, bl + ((i + 1) * widthStep), Quaternion.identity);
                Instantiate(planeMarker, tr - ((i + 1) * depthStep), Quaternion.identity);
                Instantiate(planeMarker, tr - ((i + 1) * widthStep), Quaternion.identity);
            }
        }

        Vector3 boardCentre = (bl + tr) * 0.5f; // Centre of board
        GameObject newFloor = Instantiate(floor, (bl + tr) * 0.5f, _pose.rotation);
        Vector3 floorScale = new Vector3(0.1f * Vector3.Distance(bl, tl), newFloor.transform.localScale.y, 0.1f * Vector3.Distance(bl, br));
        newFloor.transform.localScale = floorScale;

        /*Debug.DrawLine(GameProperties.BottomLeftCorner, GameProperties.TopLeftCorner, Color.blue, 1000);
        Debug.DrawLine(GameProperties.BottomLeftCorner, GameProperties.BottomRightCorner, Color.blue, 1000);
        Debug.DrawLine(GameProperties.TopRightCorner, GameProperties.TopLeftCorner, Color.blue, 1000);
        Debug.DrawLine(GameProperties.TopRightCorner, GameProperties.BottomRightCorner, Color.blue, 1000);
*/
        GameObject treeObject = GameObject.FindWithTag("Tree");
        if (treeObject == null)
        {
            Instantiate(treeModel, boardCentre, Quaternion.identity);
        }
        else
        {
            treeObject.transform.position = boardCentre;
        }
        if (!_qrDetection.lockPlane)
        {
            GetComponent<RealWorldPropertyMapper>().MapProperties(); // Assign the plane properties in GameProperties
        }
        if (planeIsMapped == false)
        {
            _gameStateManager.CalibrationSuccess();
            planeIsMapped = true;
        }
        GameObject.FindWithTag("Buddy").transform.position = boardCentre + new Vector3(0, 0.435f, 0);
    }

    /// <summary>
    /// Destroys plane and tells script to expect a new plane to be mapped
    /// </summary>
    public void ResetPlane()
    {
        ClearPlane();
        planeIsMapped = false;
    }

    /// <summary>
    /// Destroys all objects instantiated by CreateNewPlane
    /// </summary>
    public void ClearPlane()
    {
        Destroy(GameObject.FindWithTag("Floor"));
        GameObject[] existingMarkers = GameObject.FindGameObjectsWithTag("PlaneMarker");
        for (int i = 0; i < existingMarkers.Length; i++)
        {
            Destroy(existingMarkers[i]);
        }
    }
}
