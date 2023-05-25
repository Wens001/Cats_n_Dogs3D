using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : LockObjectBase
{
    public ParticleSystem VFX;
    public Sprite hintSprite;
    HingeJoint _hingeJoint;


    private void Awake()
    {
        _hingeJoint = transform.GetComponent<HingeJoint>();

        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnLock);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnUnlock);
        Messenger.AddListener(StringMgr.GameStart, BroadcastHint);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnLock);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnUnlock);
        Messenger.RemoveListener(StringMgr.GameStart, BroadcastHint);
    }


    private void OnLock(LockObjectBase obj, TouchMove lockHead)
    {
        if (obj == this)
        {
            lockHead.LockHead();
            lockHead.deathLock = true;

            VFX.Play();
            if (_hingeJoint)
            {
                lockHead.rigi.constraints = RigidbodyConstraints.None;
                _hingeJoint.connectedBody = lockHead.rigi;
            }

            Messenger.Broadcast(StringMgr.GetWinCondition);
            Messenger.Broadcast(StringMgr.HideHint, lockHead.selfType);
        }
    }

    private void OnUnlock(LockObjectBase obj)
    {
        if (obj == this)
        {
            if (_hingeJoint)
            {
                _hingeJoint.connectedBody = null;
            }

            Messenger.Broadcast(StringMgr.LoseWinCondition);
            Messenger.Broadcast(StringMgr.HintBroadcast, suitHead, hintSprite);
        }
    }

    private void BroadcastHint()
    {
        Messenger.Broadcast(StringMgr.HintBroadcast, suitHead, hintSprite);
    }


}
