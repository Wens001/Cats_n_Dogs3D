using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blower : LockObjectBase
{
    public Sprite selfHintSprite;
    public Sprite OtherHintSprite;

    public ParticleSystem WindVfx;

    private Vector3 windLocalPosition;
    private Quaternion windLocalRotation;

    HingeJoint _hingeJoint;

    private void Awake()
    {
        _hingeJoint = transform.GetComponent<HingeJoint>();
        windLocalPosition = WindVfx.transform.localPosition;
        windLocalRotation = WindVfx.transform.localRotation;


        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);

    }


    private void OnDisable()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);

    }


    #region CallBack

    private void OnHeadLocked(LockObjectBase lockObject, TouchMove lockHead)
    {
        //
        if (lockObject == this && curHead == null)
        {
            lockHead.LockHead();
            curHead = lockHead;

            /*******Test*******/
            lockHead.rigi.constraints = RigidbodyConstraints.None;
            _hingeJoint.connectedBody = lockHead.rigi;

            //提示气泡
            Messenger.Broadcast<LockObjectBase, TouchMove>(StringMgr.ShowBitIt, this, lockHead);

            //另一个头空闲时
            if (!lockHead.otherHead.IsLocked)
            {
                //提示要咬的东西
                Messenger.Broadcast(StringMgr.HintBroadcast, otherHead.selfType, OtherHintSprite);

                //显示气流特效
                WindVfx.transform.SetParent(lockHead.otherHead.transform);
                WindVfx.transform.localPosition = Vector3.zero + Vector3.forward * 1.5f;
                WindVfx.transform.rotation = WindVfx.transform.parent.rotation;
                WindVfx.gameObject.SetActive(true);
            }
            //另个头已经咬住时， deathLock
            else
            {
                Messenger.Broadcast(StringMgr.BouthDeathLock);
                WindVfx.gameObject.SetActive(false);
            }
        }

        //当另一个头咬住气球时，隐藏气流
        if (curHead != null && lockObject != this && lockHead == otherHead)
        {
            WindVfx.gameObject.SetActive(false);

        }

    }


    private void OnHeadUnlock(LockObjectBase lockObject)
    {
        if (lockObject == this)
        {
            curHead = null;
            _hingeJoint.connectedBody = null;

            WindVfx.transform.SetParent(transform);
            WindVfx.transform.localPosition = windLocalPosition;
            WindVfx.transform.localRotation = windLocalRotation;
            WindVfx.gameObject.SetActive(true);

        }




    }


    private void OnGameStart()
    {
        Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Dog, selfHintSprite);

    }



    #endregion





}
