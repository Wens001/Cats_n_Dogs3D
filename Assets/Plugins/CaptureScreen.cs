//屏幕截屏
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class CaptureScreen : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("MyTools/截屏")]
    public static void ClearPlayerPrefs()
    {
        var path = EditorUtility.SaveFilePanel(
            "Save texture as PNG",
            Application.dataPath + "/../",
            "",
            "png");
        
        if (path.Length != 0)
        {
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log(path);
        }

    }
#endif
}
