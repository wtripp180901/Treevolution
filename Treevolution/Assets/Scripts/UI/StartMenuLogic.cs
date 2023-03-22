using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.QR;

public class StartMenuLogic : MonoBehaviour
{
    public GameObject SettingsDialog;

    private void Start()
    {
        QRCodeWatcher.RequestAccessAsync();
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        //Dialog.Open(DialogPrefabGuide1, DialogButtonType.OK, "Test Dialog1", "This is Test1", true);
    }

    public void OpenSettings(GameObject caller)
    {
        Dialog d = Dialog.Open(SettingsDialog, DialogButtonType.Close, "Settings", "Lorem Ipsum", true);
        d.OnClosed = delegate (DialogResult dr) { caller.SetActive(true); };

    }
}

