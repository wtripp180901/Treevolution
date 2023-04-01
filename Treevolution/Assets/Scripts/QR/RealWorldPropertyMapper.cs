using UnityEngine;

public class RealWorldPropertyMapper : MonoBehaviour
{
    private PlaneMapper planeMapper;
    // Start is called before the first frame update

    public RealWorldPropertyMapper() { }
    public RealWorldPropertyMapper(PlaneMapper planeMapper)
    {
        this.planeMapper = planeMapper;
    }


    void Start()
    {
        if(planeMapper == null)
            planeMapper = GetComponent<PlaneMapper>();
    }
    private void Update()
    {
        if (Input.GetKeyDown("z")) GetComponent<GameStateManager>().GetComponent<GameStateManager>().BeginBattle();
    }

    public void MapProperties()
    {
        GameProperties.BottomLeftCorner = planeMapper.bottomLeft;
        GameProperties.BottomRightCorner = planeMapper.bottomRight;
        GameProperties.TopLeftCorner = planeMapper.topLeft;
        GameProperties.TopRightCorner = planeMapper.topRight;
        GameProperties.FloorHeight = planeMapper.floorHeight;
        GameProperties.Pose = planeMapper.pose;
        GameProperties.Centre = (GameProperties.BottomLeftCorner + GameProperties.TopRightCorner) * 0.5f;
    }
}
