using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 船桨
/// </summary>
public class OarAction : LockObjectBase
{
    public ParticleSystem hintVfx;
    public Sprite HintSprite;

    private Rigidbody rigi;
    private HingeJoint _hingeJoint;
    private Ship ship;

    private void Awake()
    {
        rigi = transform.GetComponent<Rigidbody>();
        _hingeJoint = GetComponent<HingeJoint>();
        ship = transform.GetComponentInParent<Ship>();
        offset = name.Contains("L") ? -1 : 1;

        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.AddListener(StringMgr.BouthLock, OnHeadsBouthLock);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.RemoveListener(StringMgr.BouthLock, OnHeadsBouthLock);

    }

    int offset;
    bool haveReady = true;
    private void OnTriggerEnter(Collider other)
    {
        if (!BouthLock)
        {
            return;
        }

        if (other.name.ToLower().Contains("back") && haveReady)
        {
            //去除提示
            //Messenger.Broadcast(StringMgr.HideHint, suitHead);

            //施加力
            ship.AddForce(offset);
            haveReady = false;
            return;
        }

        if (other.name.ToLower().Contains("forward"))
        {
            haveReady = true;
        }
        
    }



    #region CallBack

    private void OnHeadLocked(LockObjectBase lockObject, TouchMove lockHead)
    {
        if (lockObject == this && curHead == null)
        {
            lockHead.LockHead();
            curHead = lockHead;

            /*******Test*******/
            lockHead.rigi.constraints = RigidbodyConstraints.None;
            _hingeJoint.connectedBody = lockHead.rigi;
            rigi.isKinematic = false;

            //提示气泡
            Messenger.Broadcast(StringMgr.HintBroadcast, lockHead.selfType, HintSprite);

            if (hintVfx)
            {
                hintVfx.gameObject.SetActive(false);
            }


            //另个头已经咬住时， bouthLock（船桨可动）
            if (lockHead.otherHead.IsLocked)
            {
                Messenger.Broadcast(StringMgr.BouthLock);
            }
        }
    }

    private void OnHeadUnlock(LockObjectBase lockObject)
    {
        if (lockObject == this)
        {
            curHead = null;
            _hingeJoint.connectedBody = null;
            rigi.isKinematic = true;
        }
    }

    bool BouthLock = false; 
    private void OnHeadsBouthLock()
    {
        BouthLock = true;
    }

    #endregion

}
