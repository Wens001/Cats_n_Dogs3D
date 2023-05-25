using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class CheckArrived : MonoBehaviour
{
    public GameObject TargetHide;
    private List<Vector3> pointList = new List<Vector3>();
    private TouchMove[] heads;


    private void Awake()
    {
        var childList = transform.GetComponentsInChildren<Transform>().ToList();
        childList.RemoveAt(0);
        foreach (var child in childList)
        {
            pointList.Add(child.position);
        }

        heads = FindObjectsOfType<TouchMove>();
    }



    private void Update()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame && pointList.Count != 0)
        {
            for (int i = 0; i < pointList.Count; i++)
            {
                foreach (var head in heads)
                {
                    if (Vector3.Distance(pointList[i], head.transform.position) < 1.5f)
                    {
                        pointList.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            if (pointList.Count == 0)
            {
                TargetHide?.SetActive(false);
                CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);

                StartCoroutine(DelayWin());
            }
        }
    }

    IEnumerator DelayWin()
    {
        yield return new WaitForSeconds(1.5f);

        Messenger.Broadcast(StringMgr.GetWinCondition);
    }





}
