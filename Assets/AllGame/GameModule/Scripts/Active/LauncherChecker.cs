using System;
using UnityEngine;

public class LauncherChecker : MonoBehaviour
{
    [SerializeField]
    private bool isEnableCheck = true;

    void Awake()
    {
#if UNITY_EDITOR
        if (!isEnableCheck) return;
#endif
        string[] args = Environment.GetCommandLineArgs();
        bool hasLauncherFlag = false;
        foreach (string arg in args)
        {
            if (arg == "-launch_by_launcher")
            {
                hasLauncherFlag = true;
                break;
            }
        }

        if (!hasLauncherFlag)
        {
            Debug.LogError("Hãy mở game từ Launcher, không được chạy trực tiếp.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
