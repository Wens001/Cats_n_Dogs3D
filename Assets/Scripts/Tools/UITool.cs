using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITool : MonoBehaviour
{
    /// <summary>
    /// 3d物体世界坐标转屏幕坐标，在canvas是cameara模式
    /// </summary>
    /// <param name="_worldPos">物体世界坐标</param>
    /// <param name="_canvas">当前的canvas</param>
    public static Vector2 WorldToScreenWithCamrea(Vector3 _worldPos,Canvas _canvas)
    {
        Vector3 mouseDown = Camera.main.WorldToScreenPoint(_worldPos);
        Vector2 mouseUGUIPos = new Vector2();
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, mouseDown, Camera.main, out mouseUGUIPos);
        return mouseUGUIPos;
    }

    /// <summary>
    /// 3d物体世界坐标转屏幕坐标，在canvas是cameara模式
    /// </summary>
    /// <param name="_worldPos">物体世界坐标</param>
    /// <param name="_canvas">当前的canvas</param>
    /// <param name="_offset">偏移量</param>
    /// <returns></returns>
    public static Vector2 WorldToScreenWithCamrea(Vector3 _worldPos, Canvas _canvas,Vector2 _offset)
    {
        Vector3 mouseDown = Camera.main.WorldToScreenPoint(_worldPos);
        Vector2 mouseUGUIPos = new Vector2();
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, mouseDown, Camera.main, out mouseUGUIPos);
        return mouseUGUIPos+ _offset;
    }
}
