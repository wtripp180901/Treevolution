using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuLogic : MonoBehaviour
{
    public GameObject SettingsDialogPrefab;
    public GameObject StartMenu;

    private void Start()
    {
        QRCodeWatcher.RequestAccessAsync();
    }

    public void openStartMenu()
    {
        StartMenu.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings(GameObject caller)
    {
        Dialog d = Dialog.Open(SettingsDialogPrefab, DialogButtonType.Close, "Settings", "There are currently no settings available... Sorry :(", true);
        d.OnClosed = delegate (DialogResult dr) { openStartMenu(); };
    }
}

