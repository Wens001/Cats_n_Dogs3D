using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChangeCamera : MonoBehaviour
{
    public CameraType targetCamera;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TouchMove>())
        {
            CameraControl.Instance.ChangeCamera(targetCamera);
        }
    }




}
