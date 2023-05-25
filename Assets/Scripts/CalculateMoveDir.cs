using UnityEngine;

/// <summary>
/// 依据屏幕上的拖拽计算角色的移动方向
/// </summary>
public class CalculateMoveDir
{

    /// <summary>
    /// 以主相机为依据计算物体在XOZ面将要移动的方向
    /// </summary>
    /// <param name="screenDir">屏幕上的移动方向</param>
    /// <returns></returns>
    public static Vector3 CalculateDirWithCameraXOZ(Vector2 screenDir)
    {
        return Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * screenDir.y + Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * screenDir.x;
    }

    public static Vector3 CalculateDirWithCameraX(Vector2 screenDir)
    {
        return Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * screenDir.x;
    }

    public static Vector3 CalculateDirWithCameraZ(Vector2 screenDir)
    {
        return Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * screenDir.y;
    }



}
