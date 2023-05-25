using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    public Transform fan;

    public void FanRotate(float speed)
    {
        fan.Rotate(Vector3.right * speed * Time.deltaTime, Space.World);
    }
}
