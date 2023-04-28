using UnityEngine;

public class PlaneMapper : MonoBehaviour
{
    public GameObject planeMarker;
    public GameObject floor;
    public int markerCount = 2;
    public GameObject treeModel;
    public float tableWidth = 240;
    public float tableDepth = 160;


    private GameStateManager gameStateManager;
    private QRDetection qrDetection;
   
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
        planeIsMapped = planeMapped;
    }

    private void Start()
    {
        gameStateManager = GetComponent<GameStateManager>();
        qrDetection = GetComponent<QRDetection>();
    }

    public void CreateNewPlane(Pose marker)
    {
        if (qrDetection != null && qrDetection.lockPlane)
        {
            return;
        }
        if (planeIsMapped == false)
        {
            gameStateManager.CalibrationSuccess();
            planeIsMapped = true;
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

        Debug.DrawLine(GameProperties.BottomLeftCorner, GameProperties.TopLeftCorner, Color.blue, 1000);
        Debug.DrawLine(GameProperties.BottomLeftCorner, GameProperties.BottomRightCorner, Color.blue, 1000);
        Debug.DrawLine(GameProperties.TopRightCorner, GameProperties.TopLeftCorner, Color.blue, 1000);
        Debug.DrawLine(GameProperties.TopRightCorner, GameProperties.BottomRightCorner, Color.blue, 1000);

        GameObject treeObject = GameObject.FindWithTag("Tree");
        if (treeObject == null)
        {
            Instantiate(treeModel, boardCentre, Quaternion.identity);
        }
        else
        {
            treeObject.transform.position = boardCentre;
        }

        GameObject.FindWithTag("Buddy").transform.position = boardCentre + new Vector3(0, 0.435f, 0);
    }

    public void ResetPlane()
    {
        ClearPlane();
        planeIsMapped = false;
    }

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
