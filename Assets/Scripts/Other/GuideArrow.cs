using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GuideArrow : MonoBehaviour
{
    public LockObjectBase FollowObj;
    private Transform model;

    private void Awake()
    {
        model = transform.GetChild(0);
        float positionY = transform.position.y;
        model.DOMoveY(positionY + 1f, .5f).SetLoops(-1, LoopType.Yoyo);

        if (FollowObj)
        {
            transform.position = new Vector3(FollowObj.transform.position.x, transform.position.y, FollowObj.transform.position.z);
        }
        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLock);
    }


    private void OnHeadLock(LockObjectBase lockObject, TouchMove head)
    {
        if (lockObject == FollowObj)
        {
            Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLock);
            gameObject.SetActive(false);
        }

    }


    private void Update()
    {
        model.Rotate(0, 60 * Time.deltaTime, 0);
    }



}
