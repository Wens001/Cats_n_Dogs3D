using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : LockObjectBase
{
    public Sprite hintSprite;
    public ParticleSystem vfx;

    private HingeJoint _hingeJoint;

    private void Awake()
    {
        Messenger.AddListener(StringMgr.GameStart, BroadcastHint);
        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);


        _hingeJoint = transform.GetComponent<HingeJoint>();
    }
    
    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.GameStart, BroadcastHint);
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
    }


    private void BroadcastHint()
    {
        Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Default, hintSprite);
    }


    private void OnHeadLocked(LockObjectBase lockObjectBase, TouchMove lockHead)
    {
        if (lockObjectBase == this && curHead == null)
        {
            lockHead.LockHead();
            curHead = lockHead;

            if (_hingeJoint)
            {
                lockHead.rigi.constraints = RigidbodyConstraints.None;
                _hingeJoint.connectedBody = lockHead.rigi;
            }

            Messenger.Broadcast<LockObjectBase, TouchMove>(StringMgr.ShowBitIt, this, lockHead);


            //特效隐藏
            if (vfx)
            {
                vfx.gameObject.SetActive(false);
            }
        }
    }

    private void OnHeadUnlock(LockObjectBase lockObject)
    {
        if (lockObject == this)
        {
            if (_hingeJoint)
            {
                _hingeJoint.connectedBody = null;
            }

            curHead = null;
        }
    }







}
