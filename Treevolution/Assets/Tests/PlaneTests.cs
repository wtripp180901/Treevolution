using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlaneTests
    {
        [UnityTest]
        public IEnumerator TestGrassSpawning()
        {
            GameProperties.SetTestProperties(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), Vector3.one, Vector3.zero, Pose.identity, 0);
            GameObject floor = new GameObject();
            floor.AddComponent<Grass>();
            Grass grassScript = floor.GetComponent<Grass>();
            CreatorInfo grassInfo = new CreatorInfo();
            grassInfo.prefab = new GameObject();
            grassInfo.prefab.tag = "PlaneMarker";
            grassInfo.count = 10;
            grassScript.allInfo = new List<CreatorInfo>() { grassInfo };
            yield return null;
            Assert.AreEqual(11, GameObject.FindGameObjectsWithTag("PlaneMarker").Length);
        }

        [TearDown]
        public void ResetScene()
        {
            TestReseter.TearDownScene();
        }
    }
}
