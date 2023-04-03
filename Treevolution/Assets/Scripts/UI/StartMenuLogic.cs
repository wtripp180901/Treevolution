using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Logic of the StartMenu Scene.
/// </summary>
public class StartMenuLogic : MonoBehaviour
{
    /// <summary>
    /// Settings Menu Dialog Prefab.
    /// </summary>
    public GameObject SettingsDialogPrefab;
    /// <summary>
    /// Start Menu Dialog Prefab.
    /// </summary>
    public GameObject StartMenu;


    private void Start()
    {
        QRCodeWatcher.RequestAccessAsync();
    }

    /// <summary>
    /// Opens the start menu.
    /// </summary>
    public void openStartMenu()
    {
        StartMenu.SetActive(true);
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Opens the settings menu dialog, and re-opens the start menu when it is closed.
    /// </summary>
    /// <param name="caller"></param>
    public void OpenSettings()
    {
        Dialog d = Dialog.Open(SettingsDialogPrefab, DialogButtonType.Close, "Settings", "There are currently no settings available... Sorry :(", true);
        d.OnClosed = delegate (DialogResult dr) { openStartMenu(); };
    }
}

