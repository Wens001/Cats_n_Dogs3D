using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAction : MonoBehaviour
{
    public CarStatus curStatus;

    [Header("提示")]
    public Transform hintTrans;

    [Header("修理")]
    public List<SimapleLock> needsList = new List<SimapleLock>();
    public ParticleSystem smokingVFX;



    private void Awake()
    {

        Messenger.AddListener(StringMgr.BouthDeathLock, OnBouthLocked);
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
    }


    private void OnDestroy()
    {
        Messenger.RemoveListener(StringMgr.BouthDeathLock, OnBouthLocked);
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);

    }


    private void OnTriggerEnter(Collider other)
    {
        switch (curStatus)
        {
            //修理
            case CarStatus.WaitForFix:

                if (needsList.Count > 0)
                {
                    var locked = other.GetComponentInParent<SimapleLock>();
                    if (locked == needsList[0] && locked.curHead)
                    {
                        locked.gameObject.SetActive(false);
                        locked.curHead.UnlockHead();

                        needsList.RemoveAt(0);


                        if (needsList.Count > 0)
                        {
                            Messenger.Broadcast(StringMgr.otherHintBroadcast, hintTrans.gameObject, needsList[0].HintSprite);
                        }
                        else
                        {
                            if (smokingVFX)
                            {
                                smokingVFX.gameObject.SetActive(false);
                            }

                            Messenger.Broadcast(StringMgr.otherHideHint, hintTrans.gameObject);
                            Messenger.Broadcast(StringMgr.GetWinCondition);
                        }
                    }
                }



                break;
            default:
                break;
        }

    }




    #region CallBack

    private void OnGameStart()
    {
        if (curStatus == CarStatus.WaitForFix && needsList.Count > 0)
        {
            Messenger.Broadcast(StringMgr.otherHintBroadcast, hintTrans.gameObject, needsList[0].HintSprite);
        }

    }


    private void OnBouthLocked()
    {
        switch (curStatus)
        {
            case CarStatus.WaitForSafe:
                Messenger.Broadcast(StringMgr.GetWinCondition);
                break;
            case CarStatus.WaitForOil:
                StartCoroutine(AddOil());
                break;
            default:
                break;
        }
    }




    #endregion

    // 加油
    private IEnumerator AddOil()
    {
        var head = GetComponent<SimapleLock>().curHead.otherHead;
        yield return new WaitForEndOfFrame();

        int num = 0;
        while (true)
        {
            GameControl.Instance.EatThenOut(head.selfType);
            num++;
            yield return new WaitForSeconds(1);

            if (num == 2)
            {
                Messenger.Broadcast(StringMgr.GetWinCondition);
            }
        }
    }




}

public enum CarStatus
{
    none,
    WaitForSafe,
    WaitForFix,
    WaitForOil,


}

