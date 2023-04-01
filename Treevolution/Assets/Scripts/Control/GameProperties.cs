using UnityEngine;

public static class GameProperties
{
    public static Vector3 TopLeftCorner;
    public static Vector3 TopRightCorner;
    public static Vector3 BottomLeftCorner;
    public static Vector3 BottomRightCorner;
    public static Vector3 Extents;
    public static Vector3 Centre;
    public static Pose Pose;
    public static float FloorHeight;

    public static void SetTestProperties(Vector3 tlc, Vector3 trc, Vector3 blc, Vector3 brc, Vector3 ext, Vector3 cnt, Pose pose, float floor)
    {
        TopLeftCorner = tlc;
        TopRightCorner = trc;
        BottomLeftCorner = blc;
        BottomRightCorner = brc;
        Extents = ext;
        Centre = cnt;
        Pose = pose;
        FloorHeight = floor;
    }
}
