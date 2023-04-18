using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.Toolkit.UI;
using System.IO;
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
    public GameObject SettingsDialog;
    /// <summary>
    /// Start Menu Dialog Prefab.
    /// </summary>
    public GameObject StartMenu;

    private TouchScreenKeyboard _keyboard;
    private string _keyboardText = "";

    private int widthOrDepth = -1; // 0 = editing width, 1 = editing depth

    private int _tableWidthMM = 240;
    private int _tableDepthMM = 160;


    private void Update()
    {
        if(_keyboard != null && _keyboard.status != TouchScreenKeyboard.Status.Visible)
        {
            _keyboardText = _keyboard.text;
            if (widthOrDepth == 0)
            {
                try
                {
                    _tableWidthMM = int.Parse(_keyboardText);
                }
                catch { }
            }
            else if (widthOrDepth == 1)
            {
                try
                {
                    _tableDepthMM = int.Parse(_keyboardText);
                }
                catch { }
            }
            widthOrDepth = -1;
        }
    }

    /// <summary>
    /// Opens the start menu.
    /// </summary>
    public void OpenStartMenu()
    {
        StartMenu.SetActive(true);
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        GetComponent<GameStateManager>().InitRounds();
    }

    /// <summary>
    /// Opens the settings menu dialog, and re-opens the start menu when it is closed.
    /// </summary>
    /// <param name="caller"></param>
    public void OpenSettings()
    {
        SettingsDialog.SetActive(true);
    }


    public void EditTableWidth()
    {
        widthOrDepth = 0;
        _keyboard = TouchScreenKeyboard.Open(_tableWidthMM.ToString(), TouchScreenKeyboardType.NumberPad, false, false, false ,false, _tableWidthMM.ToString());
    }


    public void EditTableDepth()
    {
        widthOrDepth = 1;
        _keyboard = TouchScreenKeyboard.Open(_tableDepthMM.ToString(), TouchScreenKeyboardType.NumberPad, false, false, false, false, _tableDepthMM.ToString());

    }
    //Settings & Credits", "Click OK to change the table dimensions, or Close to go back.\nGame made by Stefan Corneci, Mingtong Du, Oscar Hewlett, Aristotelis Kostelenos, William Tripp, Kuncheng Xiao, Ash Zhuang.\nMusic by Hadi Rahmani and Billy Crook.
}

