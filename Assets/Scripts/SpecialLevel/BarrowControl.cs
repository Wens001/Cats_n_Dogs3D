using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 稻草手推车
/// </summary>
public class BarrowControl : MonoBehaviour
{
    public List<LockObjectBase> hands;
    public GameObject Scarecrows;

    private Rigidbody rigi;
    private List<LockObjectBase> lockedHands = new List<LockObjectBase>();

    private void Awake()
    {
        rigi = transform.GetComponent<Rigidbody>();
        rigi.isKinematic = true;

        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLock);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);

    }

    private void OnDisable()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLock);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
    }



    public void MoveScarecrows(Transform trarget)
    {
        Transform child = Scarecrows.transform.GetChild(0);
        if (child)
        {
            child.SetParent(null,true);
            child.localRotation = Quaternion.LookRotation(-Vector3.right);
            child.position = trarget.position;
        }

        //游戏胜利
        if (Scarecrows.transform.childCount == 0)
        {
            rigi.isKinematic = true;
            transform.GetComponent<SimapleMove>().enabled = false;
        }
    }

    #region CallBack

    private void OnHeadLock(LockObjectBase lockObject, TouchMove head)
    {
        if (hands.Contains(lockObject) && !lockedHands.Contains(lockObject))
        {
            lockedHands.Add(lockObject);

            if (lockedHands.Count == hands.Count)
            {
                Messenger.Broadcast(StringMgr.BouthDeathLock);


                //赋予简单移动
                gameObject.AddComponent<SimapleMove>().ForceRate = 500f;
                rigi.isKinematic = false;
            }
        }
    }

    private void OnHeadUnlock(LockObjectBase lockObject)
    {
        if (lockedHands.Contains(lockObject))
        {
            lockedHands.Remove(lockObject);

            SimapleMove simapleMove = GetComponent<SimapleMove>();
            if (simapleMove)
            {
                Destroy(simapleMove);
            }
        }

    }

    #endregion



}
