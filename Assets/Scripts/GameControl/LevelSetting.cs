using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetting : MonoBehaviour
{
    public CameraType cameraType;
    public CameraType SecondType;
    /// <summary>
    /// 达成胜利所需条件数
    /// </summary>
    public int WinConditionsNum;

    public bool EatLevel = false;

    public GuidType guidType;
}


public enum GuidType
{
    None,
    DoubleHandGuid,
    SlingShotGuid,

}
