#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomMenu : MonoBehaviour
{


    [MenuItem("Tools/ClearPlayerPrefs")]
    public static void ClearAllPlayerPrefs() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
#endif