using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Responisble for creating the start menu
/// </summary>
public class ManagerSceneLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        loadStartMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void loadStartMenu()
    {
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        await sceneSystem.LoadContent("StartMenu", LoadSceneMode.Additive);
    }
}
