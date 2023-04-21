using UnityEngine;
//Adapted from https://blog.sentry.io/2022/09/30/unity-exception-handling-a-beginners-guide/
public class Debugger : MonoBehaviour
{
    void Awake()
    {
        Application.logMessageReceived += LogCaughtException;
        DontDestroyOnLoad(gameObject);
    }

    void LogCaughtException(string logText, string stackTrace, LogType logType)
    {
        if (logType != LogType.Warning && logType != LogType.Log) 
        {
            GetComponent<UIController>().ShowDictation(logType.ToString()+": "+logText + "\n" + stackTrace);
        }
    }
}
