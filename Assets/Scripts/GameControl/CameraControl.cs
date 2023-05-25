using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraControl : Singleton<CameraControl>
{
    Dictionary<string, CinemachineVirtualCamera> CMs = new Dictionary<string, CinemachineVirtualCamera>();

    private bool haveInit = false;
    private CameraType curCameraType;
    private CinemachineVirtualCamera cm;

    private void Awake()
    {
        CameraControlInit();
    }

    private void CameraControlInit()
    {
        if (haveInit)
        {
            return;
        }

        haveInit = true;
        CinemachineVirtualCamera[] cinemachineVirtualCameras = transform.GetComponentsInChildren<CinemachineVirtualCamera>(true);
        foreach (var CM in cinemachineVirtualCameras)
        {
            CMs.Add(CM.name, CM);
        }
    }


    public void ChangeCamera(CameraType cameraType)
    {
        if (!haveInit)
        {
            CameraControlInit();
        }

        if (cameraType == CameraType.None)
        {
            foreach (var CM in CMs.Keys)
            {
                CMs[CM].gameObject.SetActive(false);
            }
            return;
        }


        curCameraType = cameraType;
        foreach (var CM in CMs.Keys)
        {
            var suit = CM.Equals(cameraType.ToString());
            CMs[CM].gameObject.SetActive(suit);
            if (suit)
            {
                cm = CMs[CM];
            }
            
        }
    }

    public CameraType GetCurCamera()
    {
        return curCameraType;
    }


    public void LookAtSomething(Transform _trans)
    {
        //ChangeCamera(CameraType.CM_ShopView);

        if (cm != null)
        {
            cm.Follow = _trans;
            cm.LookAt = _trans;
        }
    }




}
