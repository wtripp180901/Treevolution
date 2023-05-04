using UnityEngine;

/// <summary>
/// Responsible for mapping real world properties from an instantiated playspace to GameProperties
/// </summary>
public class RealWorldPropertyMapper : MonoBehaviour
{
    private PlaneMapper planeMapper;
    
    // Start is called before the first frame update
    void Start()
    {
        if(planeMapper == null)
            planeMapper = GetComponent<PlaneMapper>();
    }

    public void SetupForTest(PlaneMapper pm)
    {
        planeMapper = pm;
    }

    private void Update()
    {
        if (Input.GetKeyDown("z")) GetComponent<GameStateManager>().GetComponent<GameStateManager>().BeginBattle();
    }

    /// <summary>
    /// Maps properties form the plane to GameProperties
    /// </summary>
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
