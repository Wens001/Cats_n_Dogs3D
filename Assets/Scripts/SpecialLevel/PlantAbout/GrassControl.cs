using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassControl : MonoBehaviour
{
    public Sprite HintSprite;

    bool haveEnd;
    int EndCount
    {
        get
        {
            //return SDKManager.Instance.OldLevel ? 80 : 200;
            return 200;
        }
    }



    private void OnEnable()
    {
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);
    }


    private void Update()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame && !haveEnd)
        {
            if (transform.childCount < EndCount)
            {
                haveEnd = true;

                CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);
                Messenger.Broadcast(StringMgr.BouthDeathLock);
                Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Default);
                StartCoroutine(DelayWin());

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }

        }

    }

    IEnumerator DelayWin()
    {
        yield return new WaitForSeconds(.8f);
        Messenger.Broadcast(StringMgr.GetWinCondition);
    }
    



    private void OnGameStart()
    {
        Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Default, HintSprite);
        StartCoroutine("HideHint");
    }

    private IEnumerator HideHint()
    {
        yield return new WaitForSeconds(3);
        Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Default);
    }

}
